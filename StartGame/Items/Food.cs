using StartGame.PlayerData;
using System;

namespace StartGame.Items
{
    public class Food : Resource
    {
        public readonly int healAmount;

        public Food(string name, int amount, int value, int healAmount) : base(name, amount, value)
        {
            this.healAmount = healAmount;
        }

        public void UseFood(Player player)
        {
            amount--;
            player.troop.health.RawValue += healAmount;
            if(amount == 0)
            {
                player.troop.items.Remove(this);
            }
        }

        public override string Description { get => $"Amount: {amount}. It heals the player by {healAmount}."; set => throw new NotImplementedException(); }
    }

}