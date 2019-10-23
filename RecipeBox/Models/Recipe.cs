using System.Collections.Generic;

namespace RecipeBox.Models
{
  public class Recipe
  {
    public Recipe()
    {
      this.Tags = new HashSet<TagRecipe>();
      this.Ingredients = new HashSet<IngredientRecipe>();
    }

    public int RecipeId {get; set;}
    public string Name {get; set;}
    public string Description {get; set;}
    public string Instructions {get; set;}
    public int Rating {get; set;}
    public virtual ApplicationUser User {get; set;}

    public ICollection<TagRecipe> Tags {get;}
    public ICollection<IngredientRecipe> Ingredients {get;}
  }
}