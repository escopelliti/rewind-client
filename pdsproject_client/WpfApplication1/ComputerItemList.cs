using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    public class ComputerItemList : ObservableCollection<ComputerItem>, INotifyCollectionChanged
    {
        private ObservableCollection<ComputerItem> computerItemList;
        
        public ComputerItemList()
        {
            computerItemList = new ObservableCollection<ComputerItem>();
        }

        private void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
          if (this.CollectionChanged != null)
          {
            this.CollectionChanged(this, args);
          }
        }  
 
        public event NotifyCollectionChangedEventHandler CollectionChanged;           

        public void Add(ComputerItem ci) {

            computerItemList.Add(ci);
            this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, ci));
        }

        public void Remove(ComputerItem ci)
        {
            computerItemList.Remove(ci);
            this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ci));
        }

        public ComputerItem Find(Func<ComputerItem, bool> f)
        {
            return this.Where(f).ElementAt<ComputerItem>(0);
        }

    }
}
