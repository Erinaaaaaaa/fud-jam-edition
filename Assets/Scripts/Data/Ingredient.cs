using System;

namespace Data
{
    [Serializable]
    public class Ingredient
    {
        public string key;
        public string name;

        [NonSerialized] public bool IsSeed;
        [NonSerialized] public int Score = -1;
        public float seedGrowTime = 3;

        public Ingredient() {}

        public Ingredient(Ingredient other)
        {
            key = other.key;
            name = other.name;
            IsSeed = other.IsSeed;
            seedGrowTime = other.seedGrowTime;
        }
        
        protected bool Equals(Ingredient other)
        {
            return key == other.key && IsSeed == other.IsSeed;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Ingredient)obj);
        }

        public override int GetHashCode()
        {
            return (key != null ? key.GetHashCode() : 0);
        }
    }
}