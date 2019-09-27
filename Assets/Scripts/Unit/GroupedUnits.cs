namespace Assets.Scripts.Unit
{
    public class GroupedUnits
    {
        public Unit unit;
        public int amount;

        public GroupedUnits(Unit unit, int amount)
        {
            this.unit = unit;
            this.amount = amount;
        }
    }
}