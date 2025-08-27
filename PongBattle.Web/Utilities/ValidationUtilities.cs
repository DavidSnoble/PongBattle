using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PongBattle.Web.Models;

namespace PongBattle.Web.Utilities;

public class ValidationUtilities
{
    public static IActionResult ValidateFormAndRenderView(IViewModel viewModel, ModelStateDictionary modelState,
        IActionResult successAction, IActionResult failureAction, Action onSuccess)
    {
        var errors = viewModel.Validate();
        if (errors.Count == 0)
        {
            onSuccess();
            return successAction;
        }

        foreach (var error in errors) modelState.AddModelError(error.Key, error.Value);

        return failureAction;
    }
}