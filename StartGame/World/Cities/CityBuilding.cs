using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartGame.Items;

namespace StartGame.World.Cities
{
    public class CityBuilding
    {
        public static int ID = 0;
        public int id;
        public string name;
        public string description;
        public List<CityBuildingAction> actions;
        public int Priority = 10;

        public CityBuilding(int id, string name, string description, List<CityBuildingAction> actions)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.actions = actions;
        }

        public virtual void OnAction(CityBuildingAction action, CityView cityView)
        {

        }

        public override string ToString()
        {
            return name;
        }
        /// <summary>
        /// This is called once a week
        /// </summary>
        public virtual void WorldAction()
        {
            
        }
    }



    public class Port : CityBuilding
    {
        public Port(City city) : base(++ID, "Port", "It is a port.", new List<CityBuildingAction> { new CityBuildingAction("Fast Travel") })
        {
            this.city = city;
        }

        CityView cityV;
        private readonly City city;

        public override void OnAction(CityBuildingAction action, CityView cityView)
        {
            cityV = cityView;
            Trace.TraceInformation($"Store::OnAction {action.name}");
            if(action.name == "Deselect")
            {
                cityView.actionOptionList.Visible = false;
                cityView.actionOptionList.SelectedIndexChanged -= ActionOptionList_SelectedIndexChanged;
            }
            else if(action.name == "Fast Travel")
            {
                cityView.actionOptionList.Visible = true;
                cityV.actionOptionList.Items.Clear();
                foreach (var c in city.portConnections)
                {
                    cityV.actionOptionList.Items.Add(c);
                }
                cityView.actionOptionList.SelectedIndexChanged += ActionOptionList_SelectedIndexChanged;
            }
        }

        private void ActionOptionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cityV.actionOptionList.SelectedIndex == -1) {
                cityV.button1.Visible = false;
                return; }
            City selected = cityV.actionOptionList.SelectedItem as City;
            cityV.actionOptionList.Text = selected.position.ToString();
            cityV.button1.Text = "Travel";
            cityV.button1.Click += Button1_Click;
            cityV.button1.Visible = true;

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (cityV.actionOptionList.SelectedIndex == -1) return;
            City selected = cityV.actionOptionList.SelectedItem as City;
            cityV.player.WorldPosition = selected.position;
            cityV.worldView.FocusOnPlayer();
            cityV.worldView.Render();
            cityV.Close();
        }
    }
    public class CityBuildingAction
    {
        public string name;

        public CityBuildingAction(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
