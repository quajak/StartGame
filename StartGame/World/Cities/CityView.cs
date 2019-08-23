using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartGame.World.Cities
{
    public partial class CityView : Form
    {
        public readonly HumanPlayer player;
        public readonly WorldView worldView;
        CityBuilding last;

        public CityView(City city, HumanPlayer player, WorldView worldView)
        {
            InitializeComponent();
            cityName.Text = city.name;
            cityDescription.Text = $"City size: {city.GetType().Name.SplitWords()} Population: {city.Population} City Wealth: {city.Wealth}";
            playerView.Activate(player, null, false);
            this.player = player;
            this.worldView = worldView;
            foreach (var b in city.buildings)
            {
                buildingList.Items.Add(b);
            }
        }

        private void CityView_Load(object sender, EventArgs e)
        {

        }

        private void BuildingOptionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (buildingList.SelectedItem != null && buildingOptionList.SelectedItem != null)
            {
                (buildingList.SelectedItem as CityBuilding).OnAction(new CityBuildingAction("Deselect"), this);
                (buildingList.SelectedItem as CityBuilding).OnAction(buildingOptionList.SelectedItem as CityBuildingAction, this);
            }
        }

        private void BuildingList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (last != null)
                last.OnAction(new CityBuildingAction("Deselect"), this);
            buildingOptionList.Items.Clear();
            if (buildingList.SelectedIndex != -1)
            {
                buildingOptionList.Visible = true;
                buildingInfo.Visible = true;
                CityBuilding cityBuilding = buildingList.SelectedItem as CityBuilding;
                last = cityBuilding;
                buildingInfo.Text = cityBuilding.description;
                foreach (var action in cityBuilding.actions)
                {
                    buildingOptionList.Items.Add(action);
                }
            }
            else
            {
                buildingOptionList.Visible = false;
                buildingInfo.Visible = false;
            }
        }

        private void CityDescription_Click(object sender, EventArgs e)
        {

        }

        private void ActionOptionLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
