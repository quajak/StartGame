using PlayerCreator;
using StartGame.Items;
using StartGame.PlayerData;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartGame.DebugViews
{
    public partial class ItemCreator : Form
    {
        private HumanPlayer player;
        private List<CheckBox> bodyPartAffected = new List<CheckBox>();

        private List<NumericUpDown> jewelryBuffValues = new List<NumericUpDown>();
        private List<ListBox> jewelryBuffType = new List<ListBox>();

        private List<string> jewelryBuffs = new List<string>()
        {
            "Strength",
            "Agility",
            "Endurance",
            "Wisdom",
            "Intelligence"
        };

        public ItemCreator()
        {
            player = new HumanPlayer(PlayerType.localHuman, "Test", null, null, null, 10)
            {
                troop = new Troop("Test", 20, new Weapon(1, BaseAttackType.melee, BaseDamageType.sharp, 1, "Test Weapon", 1, true), Resources.playerTroop, 0, null)
                {
                    armours = new List<Armour>
                    {
                        new Armour("Woolen Tunic", 50, new List<BodyParts>{BodyParts.LeftUpperArm,BodyParts.RightUpperArm,BodyParts.Torso}, Material.Materials.First(m => m.name == "Wool"),Quality.Common, ArmourLayer.clothing),
                        new Armour("Old Pants", 40, new List<BodyParts> { BodyParts.UpperLegs, BodyParts.LeftLowerLeg, BodyParts.RightLowerLeg, BodyParts.LeftShin, BodyParts.RightShin }, Material.Materials.First(m => m.name == "Cloth"), Quality.Poor, ArmourLayer.clothing),
                        new Armour("Wooden Shoes", 32, new List<BodyParts> { BodyParts.LeftFoot, BodyParts.RightFoot }, Material.Materials.First(m => m.name == "Wood"), Quality.Poor, ArmourLayer.light)
                    }
                }
            };
            player.troop.armours.ForEach(a => a.active = true);

            InitializeComponent();
            Point pos = new Point(879, 65);
            foreach (BodyParts part in Enum.GetValues(typeof(BodyParts)))
            {
                CheckBox item = new CheckBox()
                {
                    Name = "c" + part.ToString(),
                    Text = part.ToString(),
                    Location = pos
                };
                pos.Y += 20;
                bodyPartAffected.Add(item);
                Controls.Add(item);
            }

            armourMaterialList.Items.AddRange(Enum.GetNames(typeof(Materials)));
            armourQualityList.Items.AddRange(Enum.GetNames(typeof(Quality)));
            armourLayerList.Items.AddRange(Enum.GetNames(typeof(ArmourLayer)));

            //Initialise jewelry
            pos = new Point(669, 65);
            foreach (var buff in jewelryBuffs)
            {
                Label label = new Label()
                {
                    Text = buff,
                    Location = new Point(pos.X - 50, pos.Y),
                    Width = 45
                };
                Controls.Add(label);
                NumericUpDown numericUpDown = new NumericUpDown()
                {
                    Name = "n" + buff,
                    Location = pos,
                    Width = 35,
                    Minimum = -100,
                    Maximum = 100
                };
                jewelryBuffValues.Add(numericUpDown);
                Controls.Add(numericUpDown);
                pos.X += 40;
                ListBox listBox = new ListBox()
                {
                    Name = "l" + buff,
                    Location = pos,
                    Height = 40
                };
                listBox.Items.AddRange(Enum.GetNames(typeof(BuffType)));
                pos.X -= 40;
                pos.Y += 60;
                jewelryBuffType.Add(listBox);
                Controls.Add(listBox);
            }
            jewelryType.Items.AddRange(Item.JewelryTypes.ToArray());
            jewelryMaterial.Items.AddRange(Enum.GetNames(typeof(Materials)));
            jewelryQuality.Items.AddRange(Enum.GetNames(typeof(Quality)));
            playerView.Activate(player, null, false);
        }

        private void ArmourCreator_Load(object sender, EventArgs e)
        {
        }

        private void CreateArmour_Click(object sender, EventArgs e)
        {
            string name = armourName.Text;
            if (name == "")
            {
                return;
            }
            List<BodyParts> affected = new List<BodyParts>();
            foreach (CheckBox bodyTick in bodyPartAffected)
            {
                if (bodyTick.Checked)
                {
                    Enum.TryParse(bodyTick.Text, out BodyParts part);
                    affected.Add(part);
                }
            }
            if (affected.Count == 0) return;

            if ((string)armourMaterialList.SelectedItem == "") return;
            Material material = Material.Materials.First(m => m.name == (string)armourMaterialList.SelectedItem);

            if ((string)armourQualityList.SelectedItem == "") return;
            Quality quality = (Quality)Enum.Parse(typeof(Quality), (string)armourQualityList.SelectedItem);

            if ((string)armourLayerList.SelectedItem == "") return;
            ArmourLayer layer = (ArmourLayer)Enum.Parse(typeof(ArmourLayer), (string)armourLayerList.SelectedItem);

            Armour armour = new Armour(name, (int)baseArmourDurability.Value, affected, material, quality, layer);
            string affectedParts = "new List<BodyParts>{";
            foreach (var item in affected)
            {
                affectedParts += "BodyParts." + item.ToString() + ",";
            }
            affectedParts = affectedParts.Substring(0, affectedParts.Length - 1);
            affectedParts += "}";
            output.Text = $"Armour {name} = new Armour(\"{name}\", {(int)baseArmourDurability.Value}, {affectedParts}, Material.Materials.First(m => m.name == \"{material.name}\")," +
                $"Quality.{quality.ToString()}, ArmourLayer.{layer.ToString()});";
            player.troop.armours.Add(armour);
            playerView.Render();
        }

        private void ArmourLayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void JewelryCreator_Click(object sender, EventArgs e)
        {
            string BuffString(Buff buff)
            {
                return $"new Buff(BuffType.{buff.type}, {buff.value}) ";
            }

            if (jewelryName.Text == "") return;
            string name = jewelryName.Text;

            Buff strength = new Buff(0, 2);
            Buff agility = new Buff(0, 2);
            Buff wisdom = new Buff(0, 2);
            Buff intelligence = new Buff(0, 2);
            Buff endurance = new Buff(0, 2);
            try
            {
                foreach (var buff in jewelryBuffs)
                {
                    int value = (int)(Controls.Find("n" + buff, false).First() as NumericUpDown).Value;
                    if (value == 0) continue;
                    BuffType type = (BuffType)Enum.Parse(typeof(BuffType), (string)(Controls.Find("l" + buff, false).First() as ListBox).SelectedItem);
                    switch (buff)
                    {
                        case "Strength":
                            strength = new Buff(type, value);
                            break;

                        case "Agility":
                            agility = new Buff(type, value);
                            break;

                        case "Wisdom":
                            wisdom = new Buff(type, value);
                            break;

                        case "Intelligence":
                            intelligence = new Buff(type, value);
                            break;

                        case "Endurance":
                            endurance = new Buff(type, value);
                            break;

                        default:
                            throw new Exception();
                    }
                }
            }
            catch (Exception E)
            {
                return;
            }
            if ((string)jewelryMaterial.SelectedItem == "") return;
            Material material = Material.Materials.First(m => m.name == (string)jewelryMaterial.SelectedItem);

            if ((string)jewelryQuality.SelectedItem == "") return;
            Quality quality = (Quality)Enum.Parse(typeof(Quality), (string)jewelryQuality.SelectedItem);

            if (jewelryType.SelectedItem is null) return;
            JewelryType jtype = jewelryType.SelectedItem as JewelryType;
            Jewelry jewelry = Jewelry.New(strength, intelligence, wisdom, agility, endurance, name, quality, material, jtype);
            output.Text = $"Jewelry.New({BuffString(strength)},{BuffString(intelligence)},{BuffString(wisdom)},{BuffString(agility)},{BuffString(endurance)},\"{name}\", " +
                $"Quality.{quality}, Material.Materials.First(m => m.name == \"{material.name}\"), Item.GetJewelryType(\"{jtype}\"));";
            jewelry.Player = player;
            player.troop.jewelries.Add(jewelry);
            playerView.Render();
        }
    }
}