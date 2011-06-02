using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Infragistics.Win.UltraWinTree;
using TourWriter.Forms;

namespace TourWriter.Services
{
    class TabHistory
    {
        private readonly List<UltraTreeNode> _formNodes = new List<UltraTreeNode>();

        public List<UltraTreeNode> FormNodes
        {
            get { return _formNodes; }
        }

        public void Add(UltraTreeNode node)
        {
            var existing = _formNodes.Where(x => x.Key == node.Key);
            if (existing.Count() == 0) 
                _formNodes.Add(node);
            ActiveIndex = _formNodes.Count() - 1;
        }

        public void Remove(UltraTreeNode node)
        {
            var t = _formNodes.Where(x => x.Key == node.Key).FirstOrDefault();
            if (t != null) _formNodes.Remove(t);
        }

        public void Reposition(UltraTreeNode node, int index)
        {
            Remove(node);
            _formNodes.Insert(index, node);
            SetActiveNode(node);
        }

        public void SetActiveNode(UltraTreeNode node)
        {
            var t = _formNodes.Where(x => x.Key == node.Key).FirstOrDefault();
            if (t != null) ActiveIndex = t.Index;
        }

        public int? ActiveIndex { get; private set; }

        public string ToJson()
        {
            var t = new Json { Index = ActiveIndex, Keys = _formNodes.Where(x => x != null).Select(x => x.Key).ToArray() };
            var json = new JavaScriptSerializer().Serialize(t);
            return json;
        }

        public void LoadFromJson(string json)
        {
            var js = new JavaScriptSerializer().Deserialize<Json>(json);
            if (js == null) return;

            ActiveIndex = js.Index;
            _formNodes.Clear();
            foreach (var key in js.Keys.Where(x => x != null))
            {
                var node = MenuHelper.Menu_FindNode(key);
                Add(node ?? new UltraTreeNode(key));
            }
        }

        [Serializable]
        class Json
        {
            public int? Index;
            public string[] Keys;
        }
    }
}
