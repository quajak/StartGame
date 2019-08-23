using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using StartGame.Items;
using StartGame.PlayerData;

namespace StartGame.World.Cities
{
    public abstract class Producer : CityBuilding, IProducer
    {
        internal List<Resource> items = new List<Resource>();
        internal int workers;
        internal int money = 100;
        readonly Dictionary<string, int> itemsWanted = new Dictionary<string, int>(); //For last year
        readonly Dictionary<string, int> itemsProduced = new Dictionary<string, int>(); //For last year
        public Producer(int id, string name, string description, List<CityBuildingAction> actions) : base(id, name, description, actions)
        {

        }

        internal void AddItem(Resource resource)
        {
            if(items.TryGet(r => r.name == resource.name, out Resource item))
            {
                item.amount += resource.amount;
            }
            else
            {
                items.Add(resource);
            }
        }

        internal void ProduceItem(string name, int amount)
        {
            int price = Nation.prices.Find(s => s.name == name).cost;
            if(itemsWanted.ContainsKey(name) && itemsProduced.ContainsKey(name))
            {
                price = (int)(price * (double)(itemsWanted[name] - itemsProduced[name]) / itemsProduced[name]);
            }
            //Trace.TraceInformation($"{this.name}: Produced {name} x{amount} at {price} by {workers}");
            AddItem(new Resource(name, amount, price));
        }

        public List<Resource> Items()
        {
            return items;
        }
        public int Workers()
        {
            return workers;
        }

        public int Money() {
            return money;
        }
    }

    public interface IProducer
    {
        int Workers();

        List<Resource> Items();

        int Money();
    }

    public class Logger : Producer
    {
        private readonly City city;
        CityView cityV;
        HumanPlayer player;

        public Logger(City city) : base(++ID, "Logger", "The logger works in the sorrounding area and cuts wood", new List<CityBuildingAction> { new CityBuildingAction("Work") })
        {
            this.city = city;
            workers = city.Population / 100; //TODO: Get a better way to handle the population
        }

        public override void OnAction(CityBuildingAction action, CityView cityView)
        {
            cityV = cityView;
            player = cityV.player;
            Trace.TraceInformation($"Store::OnAction {action.name}");
            if (action.name == "Deselect")
            {
                cityV.actionOptionLabel.Visible = false;
                cityV.button1.Visible = false;
            }
            else if (action.name == "Work")
            {
                cityV.actionOptionLabel.Visible = false;
                Skill skill = (player.GetTree("Woodcutter") as Skill);
                int payment = GetPayment(skill);
                cityV.actionOptionLabel.Text = $"You can work for the day as a logger for {payment} coins.";
                cityV.button1.Visible = true;
                cityV.button1.Text = "Work";
                cityV.button1.Click += Button1_Click;

            }
        }

        private static int GetPayment(Skill skill)
        {
            return (int)(20 * (1d + (skill?.level ?? 0) / 20d));
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            cityV.Close();
            bool running = cityV.worldView.controller.Enabled;
            PlayerWork();
            if (running)
                cityV.worldView.controller.Start();
        }

        private void PlayerWork()
        {
            World.Instance.ProgressTime(new TimeSpan(8, 0, 0));
            Skill s = player.GetTree("Woodcutter") as Skill;
            int payment = GetPayment(s);
            string text = $"You have finished working for the logger and been payed {payment} coins.";
            if (s is null)
            {
                s = new Woodcutter();
                player.trees.Add(s);
                s.Activate();
                text += " You have also gained the woodcutter skill.";
            }
            else
            {
                s.Xp += 1;
                text += " You have gained 1 Xp in the woodcutter skill.";
            }
            player.Money.RawValue += payment;
            player.playerView.Render();
            ProduceItem("Wood", 2);
            cityV.worldView.controller.Stop();
            text += " Do you want to continue working for the logger?";
            if (MessageBox.Show(text, "Logger", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                PlayerWork();
            }
        }

        int week = 0;
        public override void WorldAction()
        {
            week++;
            if (week % 51 != 0)
                return;
            //Find closest tile to work
            Point point = city.position;
            int searchSize = 1;
            WorldTile toWork = null;
            while (true)
            {
                double maxForest = -0.1;
                for (int x = 0; x < searchSize; x++)
                {
                    for (int y = 0; y < searchSize; y++)
                    {
                        WorldTile tile = World.Instance.worldMap.Get((x + point.X).Cut(0, World.WORLD_SIZE - 1), (y + point.Y).Cut(0, World.WORLD_SIZE - 1));
                        if (tile.forest > maxForest)
                        {
                            toWork = tile;
                            maxForest = tile.forest;
                        }
                    }
                }
                if (maxForest > 0.2)
                {
                    break;
                }
                if (searchSize == 10)
                    break;
                searchSize++;
            }
            double work = (double)workers * (5 - AIUtility.Distance(toWork.position, point)).Cut(1, 6) / 50 * (1.25 - World.random.NextDouble() / 2);
            toWork.forest -= work;
            toWork.forest = toWork.forest.Cut(0, 100);
            ProduceItem("Wood", (int)(work * 10));
        }
    }
}
