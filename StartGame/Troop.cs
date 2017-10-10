using System.Drawing;

namespace StartGame
{
    internal class Troop
    {
        public bool[,] form;
        public TroopTypeEnum type;
        public string name;

        public Troop(bool[,] Form, TroopTypeEnum Type, string Name)
        {
            form = Form;
            type = Type;
            name = Name;
        }
    }

    public enum TroopTypeEnum : int { infantry, cavalary, air, building, empty }

    internal struct TroopType
    {
        public TroopTypeEnum troopType;
        public Color fillColor;
        public Color borderColor;
    }
}