using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace RecipeBox.Controllers
{
  [Authorize]
  public class RecipesController: Controller
  {
    private readonly RecipeBoxContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public RecipesController(UserManager<ApplicationUser> userManager, RecipeBoxContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userRecipes = _db.Recipes.Where(recipe => recipe.User.Id == currentUser.Id);
      return View(userRecipes);
    }

    public ActionResult Create()
    {
      ViewBag.TagId = new SelectList(_db.Tags, "TagId", "Name");
      ViewBag.IngredientId = new SelectList(_db.Ingredients, "IngredientId", "Name");
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Recipe recipe, int TagId, int IngredientId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      recipe.User = currentUser;
      _db.Recipes.Add(recipe);
      if (TagId != 0)
      {
        _db.TagRecipe.Add(new TagRecipe() {TagId = TagId, RecipeId = recipe.RecipeId});
      }
            if (TagId != 0)
      {
        _db.IngredientRecipe.Add(new IngredientRecipe() {IngredientId = IngredientId, RecipeId = recipe.RecipeId});
      }
      _db.SaveChanges();
      TempData["RecipeId"] = recipe.RecipeId;
      return RedirectToAction("Details");
    }

    public ActionResult Details (int id)
    {
      var CreatedRecipeId = TempData["RecipeId"] ;
      if (id == 0)
      {
        id = (int)CreatedRecipeId;
      }
      var thisRecipe = _db.Recipes
        .Include(recipe => recipe.Tags)
        .ThenInclude(join => join.Tag)
        .Include(recipe => recipe.Ingredients)
        .ThenInclude(join => join.Ingredient)
        .FirstOrDefault(recipe => recipe.RecipeId == id);
      return View(thisRecipe);
    }


  }
}