using System.Collections;

namespace WallyInterpreter.Components.Interpreter.Parser
{
    public class ItemCollectionLR0 : IItemCollectionLR0
    {
        private List<IItemLR0> _items;
        private string _id;
        public ItemCollectionLR0(IItemLR0[] items)
        {
            _items = new List<IItemLR0>(items);
            _items.Sort((IItemLR0 a, IItemLR0 b) => {
                return a.ID.CompareTo(b.ID);
            });
            _id = _items[0].ID;
            foreach (var item in _items) {
                _id += " - "+item.ID;
            }

        }
        public string ID =>_id;

        public IEnumerator<IItemLR0> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
