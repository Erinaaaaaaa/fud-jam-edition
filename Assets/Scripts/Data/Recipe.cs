using System;

namespace Data
{
    [Serializable]
    public class Recipe
    {
        public Ingredient[] ingredients;
        public Ingredient product;

        protected bool Equals(Recipe other)
        {
            return Equals(ingredients, other.ingredients) && Equals(product, other.product);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Recipe)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ingredients, product);
        }
    }
}