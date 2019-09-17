namespace PiRhoSoft.Composition.Editor
{
	public class ObjectAutocompleteItem : AutocompleteItem
	{
		public ObjectAutocompleteItem(object obj) => Setup(obj);

		public override void Setup(object obj)
		{
		}
	}

	public abstract class ObjectAutocompleteItem<T> : ObjectAutocompleteItem where T : class
	{
		public ObjectAutocompleteItem(T obj) : base(obj) { }

		public override void Setup(object obj)
		{
			base.Setup(obj);
			Setup(obj as T);
		}

		protected abstract void Setup(T obj);
	}
}
