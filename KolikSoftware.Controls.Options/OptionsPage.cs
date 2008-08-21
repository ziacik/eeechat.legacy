using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Drawing;

namespace KolikSoftware.Controls.Options
{
    public class OptionsPage
    {
        public OptionsPage()
        {
        }

        string caption = "Default";

        public string Caption
        {
            get
            {
                return this.caption;
            }
            set
            {
                this.caption = value;
            }
        }

        private Image image = Properties.Resources.OptionsGeneral;

        public Image Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
            }
        }
    }

    public class OptionsPageList : ArrayList, IList<OptionsPage>
    {
        public override int Add(object value)
        {
            int returnValue = base.Add(value);
            OnAdded(new AddedEventArgs(value as OptionsPage));
            return returnValue;
        }

        public override void Remove(object obj)
        {
            OnRemoved(new RemovedEventArgs(obj as OptionsPage));
            base.Remove(obj);
        }

        public override void RemoveAt(int index)
        {
            OnRemoved(new RemovedEventArgs(this[index]));
            base.RemoveAt(index);
        }

        public override void RemoveRange(int index, int count)
        {
            OnRangeRemoved(new RangeRemovedEventArgs(index, count));
            base.RemoveRange(index, count);
        }

        public override void Clear()
        {
            OnCleared(ClearedEventArgs.Empty);
            base.Clear();
        }

        #region Events
        public class AddedEventArgs : EventArgs
        {
            private OptionsPage page;

            public OptionsPage Page
            {
                get
                {
                    return this.page;
                }
            }

            public AddedEventArgs(OptionsPage page)
            {
                this.page = page;
            }
        }

        public event EventHandler<AddedEventArgs> Added;

        protected virtual void OnAdded(AddedEventArgs e)
        {
            EventHandler<AddedEventArgs> handler = Added;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class RemovedEventArgs : EventArgs
        {
            private OptionsPage page;

            public OptionsPage Page
            {
                get
                {
                    return this.page;
                }
            }

            public RemovedEventArgs(OptionsPage page)
            {
                this.page = page;
            }
        }

        public event EventHandler<RemovedEventArgs> Removed;

        protected virtual void OnRemoved(RemovedEventArgs e)
        {
            EventHandler<RemovedEventArgs> handler = Removed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class ClearedEventArgs : EventArgs
        {
            public static readonly new ClearedEventArgs Empty = new ClearedEventArgs();
        }

        public event EventHandler<ClearedEventArgs> Cleared;

        protected virtual void OnCleared(ClearedEventArgs e)
        {
            EventHandler<ClearedEventArgs> handler = Cleared;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class RangeRemovedEventArgs : EventArgs
        {
            public RangeRemovedEventArgs(int index, int count)
            {
                this.index = index;
                this.count = count;
            }

            private int index;

            public int Index
            {
                get
                {
                    return this.index;
                }
            }

            private int count;

            public int Count
            {
                get
                {
                    return this.count;
                }
            }
        }

        public event EventHandler<RangeRemovedEventArgs> RangeRemoved;

        protected virtual void OnRangeRemoved(RangeRemovedEventArgs e)
        {
            EventHandler<RangeRemovedEventArgs> handler = RangeRemoved;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region IList<OptionsPage> Members
        public int IndexOf(OptionsPage item)
        {
            return base.IndexOf(item);
        }

        public void Insert(int index, OptionsPage item)
        {
            base.Insert(index, item);
            OnAdded(new AddedEventArgs(item));
        }

        public new OptionsPage this[int index]
        {
            get
            {
                return base[index] as OptionsPage;
            }
            set
            {
                base[index] = value;
            }
        }

        #endregion

        #region ICollection<OptionsPage> Members

        public void Add(OptionsPage item)
        {
            base.Add(item);
            OnAdded(new AddedEventArgs(item));
        }

        public bool Contains(OptionsPage item)
        {
            return base.Contains(item);
        }

        public void CopyTo(OptionsPage[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Remove(OptionsPage item)
        {
            OnRemoved(new RemovedEventArgs(item));
            base.Remove(item);
            return true;
        }

        #endregion

        #region IEnumerable<OptionsPage> Members

        public new IEnumerator<OptionsPage> GetEnumerator()
        {
            return (IEnumerator<OptionsPage>)base.GetEnumerator();
        }

        #endregion
    }
}
