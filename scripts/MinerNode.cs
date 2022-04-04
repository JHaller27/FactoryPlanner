using System.ComponentModel;
using FactoryPlanner.scripts.machines;
using Godot;

public class MinerNode : EfficientMachineNode
{
	private OptionButton MkOptionButton => this.ControlsContainer.GetChild<HBoxContainer>(2).GetChild<OptionButton>(0);
	private OptionButton PurityOptionButton => this.ControlsContainer.GetChild<HBoxContainer>(2).GetChild<OptionButton>(1);
	private OptionButton ResourceOptionButton => this.ControlsContainer.GetChild<OptionButton>(3);

	private enum LevelList
	{
		[Description("Mk. 1")]
		Mk1,

		[Description("Mk. 2")]
		Mk2,

		[Description("Mk. 3")]
		Mk3,
	}

	private enum PurityList
	{
		Pure,
		Normal,
		Impure,
	}

	private LevelList Level { get; set; } = LevelList.Mk1;
	private PurityList Purity { get; set; } = PurityList.Normal;

	public MinerNode() : base(0, 1)
	{
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		AddEnumItems(this.MkOptionButton, typeof(LevelList));
		AddEnumItems(this.PurityOptionButton, typeof(PurityList), 1);

		foreach (Recipe recipe in Recipe.GetRecipesWithTags("Miner"))
		{
			AddOption(this.ResourceOptionButton, recipe.Name, recipe.Id);
		}

		this._on_Resource_Selected(0);
	}

	protected override Recipe ChooseRecipe()
	{
		Recipe recipe = base.ChooseRecipe();

		// Modify for Mk
		recipe = recipe.ModifyOutputCapacity(0, cap =>
		{
			switch (this.Level)
			{
				// Modify for purity
				case LevelList.Mk2:
					return cap * 2;
				case LevelList.Mk3:
					return cap * 4;
				default:
					return cap;
			}
		});

		// Modify for purity
		recipe = recipe.ModifyOutputCapacity(0, cap =>
		{
			switch (this.Purity)
			{
				// Modify for purity
				case PurityList.Pure:
					return cap * 2;
				case PurityList.Impure:
					return cap / 2;
				default:
					return cap;
			}
		});

		return recipe;
	}

	private void _on_Resource_Selected(int index)
	{
		this.RecipeId = (string)this.ResourceOptionButton.GetItemMetadata(index);
		this.UpdateRecipe();
	}

	private void _on_MkOptionButton_item_selected(int index)
	{
		this.Level = (LevelList)this.MkOptionButton.GetItemMetadata(index);
		this.UpdateRecipe();
	}

	private void _on_PurityOptionButton2_item_selected(int index)
	{
		this.Purity = (PurityList)this.PurityOptionButton.GetItemMetadata(index);
		this.UpdateRecipe();
	}
}
