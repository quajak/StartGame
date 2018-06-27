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
    public partial class ArmourCreator : Form
    {
        private HumanPlayer player;
        private List<CheckBox> bodyPartAffected = new List<CheckBox>();

        public ArmourCreator()
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
    }
}