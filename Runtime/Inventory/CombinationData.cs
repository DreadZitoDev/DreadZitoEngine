using System.Collections.Generic;
using System.Linq;

namespace DreadZitoEngine.Runtime.Inventory
{
    public class CombinationData
    {
        public List<ItemDataSO> Items = new List<ItemDataSO>();
        public CombinationRecipeSO[] PossibleRecipes;
        public CombinationState State;
        
        public bool IsDone => State == CombinationState.Done;
        public bool IsContinue => State == CombinationState.Continue;
        public bool IsCancel => State == CombinationState.Cancel;
        
        public ItemDataSO Result;

        public void EvaluateAddItem(ItemDataSO item)
        {
            Items.Add(item);
            var state = CombinationState.None;
            foreach (var recipe in PossibleRecipes)
            {
                state = CheckCombination(recipe);
                if (state == CombinationState.Done) {
                    Result = recipe.Result;
                    break;
                }
                else if (state == CombinationState.Continue)
                {
                    break;
                }
                else if (state == CombinationState.Cancel)
                {
                    
                }
            }

            State = state;
        }
        
        private CombinationState CheckCombination(CombinationRecipeSO recipe)
        {
            // evaluate if current combination is filling a recipe
            
            var hasItems = recipe.Items.Any(item => Items.Contains(item));
            var hasItemsThatNotContains = Items.Any(item => !recipe.Items.Contains(item));
            var hasDuplicateItems = Items.GroupBy(item => item).Any(group => group.Count() > 1);
            if (hasItems && !hasItemsThatNotContains && !hasDuplicateItems)
            {
                if (recipe.Items.Length == Items.Count)
                {
                    return CombinationState.Done;
                }
                return CombinationState.Continue;
            }
            
            return CombinationState.Cancel;
        }
        
        public CombinationData Copy()
        {
            return new CombinationData()
            {
                Items = new List<ItemDataSO>(Items),
                PossibleRecipes = new List<CombinationRecipeSO>(PossibleRecipes).ToArray(),
                State = State
            };
        }
    }
}