namespace Assets.Scripts.Resources
{
    public abstract class ResourceType
    {
        public float amount;

        protected ResourceType(float amount) => this.amount = amount;
        protected ResourceType() => amount = 0;
    }

    public class Wood : ResourceType
    {
        public Wood(float amount) : base(amount) { }
        public Wood() => amount = 0;
    }

    public class Gold : ResourceType
    {
        public Gold(float amount) : base(amount) { }
        public Gold() => amount = 0;
    }

    public class Food : ResourceType
    {
        public Food(float amount) : base(amount) { }
        public Food() => amount = 0;
    }

    public class Rock : ResourceType
    {
        public Rock(float amount) : base(amount) { }
        public Rock() => amount = 0;
    }
}