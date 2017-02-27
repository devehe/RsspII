/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-24 8:53:10 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading;
using System.ComponentModel;
using System.Reflection;

namespace BJMT.RsspII4net.Utilities
{
    /// <summary>
    /// �̰߳�ȫ��������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ThreadSafetyList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    {
        #region __Enumerator1��
        /// <summary>
        /// ����ö�ٽӿ�(֧���ڷ��ͼ����Ͻ��м򵥵���),ʵ����ö�ٹ�����breakʱ�ͷŻ���
        /// </summary>
        internal class __Enumerator1 : IEnumerator<T>, IDisposable//, IEnumerator
        {
            /// <summary>
            /// ��ǰ״̬
            /// </summary>
            int __state;
            /// <summary>
            /// ��ǰö�ٵ��ĳ�Ա
            /// </summary>
            T __current;
            /// <summary>
            /// ThreadSafetyList��ʵ��
            /// </summary>
            ThreadSafetyList<T> __this;
            /// <summary>
            /// ö�ٵ�����
            /// </summary>
            int i;            

            /// <summary>
            /// Track whether Dispose has been called.
            /// </summary>
            private bool disposed = false;

            #region __Enumerator1
            /// <summary>
            /// ���캯��
            /// </summary>
            /// <param name="__this">ThreadSafetyList�����</param>
            internal __Enumerator1(ThreadSafetyList<T> __this)
            {
                this.__this = __this;
            }
            #endregion
            
            
            #region ��������
            /// <summary>
            /// ��������
            /// </summary>
            ~__Enumerator1()
            {
                Dispose(false);
            }
            #endregion

            #region Current����
            /// <summary>
            /// ��ȡ������λ��ö������ǰλ�õ�Ԫ��
            /// </summary>
            public T Current
            {
                get { return __current; }
            }
            #endregion

            #region IEnumerator.Current����
            /// <summary>
            /// ��ȡ�����еĵ�ǰԪ��
            /// </summary>
            object IEnumerator.Current
            {
                get { return __current; }
            }
            #endregion

            #region MoveNext����
            /// <summary>
            /// ��ö�����ƽ������ϵ���һ��Ԫ��
            /// </summary>
            /// <returns>���ö�����ɹ����ƽ�����һ��Ԫ�أ���Ϊ true�����ö����Խ�����ϵĽ�β����Ϊ false</returns>
            public bool MoveNext()
            {
                switch (__state)
                {
                    case 1: goto __state1;
                    case 2: goto __state2;
                }
                i = 0;
            __loop:
                if (i >= __this.Count) goto __state2;
                __current = __this[i];
                __state = 1;
                return true;
            __state1:
                ++i;
                goto __loop;
            __state2:
                __state = 2;
                return false;
            }
            #endregion

            #region Dispose����
            /// <summary>
            /// �ͷ�ʹ����Դ
            /// </summary>
            public void Dispose()
            {
                //__state = 2;
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            #endregion

            #region ����Dispose
            protected virtual void Dispose(bool disposing)
            {
                if (!this.disposed)
                {                   
                    // If disposing equals true, dispose all managed 
                    // and unmanaged resources.
                    if (disposing)
                    {
                        // Dispose managed resources.                    
                    }
                    // Release unmanaged resources. If disposing is false, 
                    // only the following code is executed.
                    //CloseHandle(handle);
                }
                disposed = true;
            }
            #endregion

            #region Reset����
            /// <summary>
            /// ��ö��������Ϊ���ʼλ�ã���λ��λ�ڼ����е�һ��Ԫ��֮ǰ
            /// </summary>
            public void Reset()
            {
                __state = 0;
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// ���������
        /// </summary>
        private List<T> _list;
        /// <summary>
        /// ���ڿ�����������������Ķ���
        /// </summary>
        private object _syncRoot = new object();                   

        #region ���캯��
        /// <summary>
        /// ��ʼ��.ThreadSafetyList�����ʵ������ʵ��Ϊ�ղ��Ҿ���Ĭ�ϳ�ʼ����.
        /// </summary>
        public ThreadSafetyList()
        {
            _list = new List<T>();            
        }

        /// <summary>
        /// ��ʼ��ThreadSafetyList�����ʵ������ʵ��������ָ�����ϸ��Ƶ�Ԫ�ز��Ҿ����㹻�����������������Ƶ�Ԫ�ء�
        /// </summary>
        /// <param name="collection">һ�����ϣ���Ԫ�ر����Ƶ����б���</param>
        public ThreadSafetyList(IEnumerable<T> collection)
        {
            _list = new List<T>(collection);
        }

        /// <summary>
        /// ��ʼ��ThreadSafetyList�����ʵ������ʵ��Ϊ�ղ��Ҿ���ָ���ĳ�ʼ������
        /// </summary>
        /// <param name="capacity">���б�������Դ洢��Ԫ����</param>
        public ThreadSafetyList(int capacity)
        {
            _list = new List<T>(capacity);
        }
        #endregion

        #region Capacity
        /// <summary>
        /// ��ȡ�����ø��ڲ����ݽṹ�ڲ�������С��������ܹ������Ԫ������
        /// </summary>
        public int Capacity
        {
            get { return _list.Capacity; }
            set { _list.Capacity = value; }
        }
        #endregion

        #region Count
        /// <summary>
        /// ��ȡThreadSafetyList��ʵ�ʰ�����Ԫ����
        /// </summary>
        public int Count
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return _list.Count;
                }
            }
        }
        #endregion

        #region this
        /// <summary>
        /// ��ȡ������ָ����������Ԫ�ء�
        /// </summary>
        /// <param name="index"> Ҫ��û����õ�Ԫ�ش��㿪ʼ��������</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                lock (this.SyncRoot) { return _list[index]; }
            }

            set
            {
                lock (this.SyncRoot) { _list[index] = value; }
            }
        }
        #endregion
        
        #region Add(T item) 
        /// <summary>
        /// ��������ӵ� ThreadSafetyList�Ľ�β����
        /// </summary>
        /// <param name="item">Ҫ��ӵ�ĩβ���Ķ��󡣶����������ͣ���ֵ����Ϊnull</param>
        public void Add(T item)
        {
            lock(this.SyncRoot)
            {
                _list.Add(item);
            }
        }
        #endregion

        #region AddRange(IEnumerable<T> collection) 
        /// <summary>
        /// ��ָ�����ϵ�Ԫ����ӵ� ThreadSafetyList��ĩβ��
        /// </summary>
        /// <param name="collection"> һ�����ϣ���Ԫ��Ӧ����ӵ�ThreadSafetyList ��ĩβ������������Ϊnull���������԰���Ϊnull��Ԫ�أ�������� T Ϊ�������ͣ�</param>
        public void AddRange(IEnumerable<T> collection)
        {
            lock (this.SyncRoot)
            {
                _list.AddRange(collection);
            }
        }
        #endregion

        #region AsReadOnly() 
        /// <summary>
        /// �ص�ǰ���ϵ�ֻ��ThreadSafetyList��װ
        /// </summary>
        /// <returns>��Ϊ��ǰ ThreadSafetyList��Χ��ֻ����װ�� System.Collections.Generic.ReadOnlyCollection</returns>
        public ReadOnlyCollection<T> AsReadOnly()
        {
            lock (this.SyncRoot)
            {
                return _list.AsReadOnly();
            }
        }
        #endregion

        #region BinarySearch
        /// <summary>
        /// ʹ��Ĭ�ϵıȽ����������������ThreadSafetyList������Ԫ�أ������ظ�Ԫ�ش��㿪ʼ��������
        /// </summary>
        /// <param name="item">Ҫ��λ�Ķ��󡣶����������ͣ���ֵ����Ϊnull</param>
        /// <returns>����ҵ� item����Ϊ�������ThreadSafetyList�� item �Ĵ��㿪ʼ������������Ϊһ���������ø����Ǵ���
        ///     item �ĵ�һ��Ԫ�ص������İ�λ�󲹡����û�и����Ԫ�أ���ΪThreadSafetyList.Count�İ�λ�󲹡�</returns> 
        public int BinarySearch(T item)
        {
            lock (this.SyncRoot)
            {
                return _list.BinarySearch(item);
            }
        }

        /// <summary>
        /// ʹ��ָ���ıȽ����������������ThreadSafetyList������Ԫ�أ������ظ�Ԫ�ش��㿪ʼ������
        /// </summary>
        /// <param name="item">Ҫ��λ�Ķ��󡣶����������ͣ���ֵ����Ϊnull</param>
        /// <param name="comparer">�Ƚ�Ԫ��ʱҪʹ�õ� System.Collections.Generic.IComparerʵ�֡�- �� - Ϊnull ��ʹ��Ĭ�ϱȽ���
        /// ystem.Collections.Generic.Comparer.Default</param>
        /// <returns>����ҵ� item����Ϊ�������ThreadSafetyList�� item �Ĵ��㿪ʼ������������Ϊһ���������ø����Ǵ���
        ///     item �ĵ�һ��Ԫ�ص������İ�λ�󲹡����û�и����Ԫ�أ���ΪThreadSafetyList.Count�İ�λ�󲹡�</returns>
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            lock (this.SyncRoot)
            {
                return _list.BinarySearch(item, comparer);
            }
        }

        /// <summary>
        /// ʹ��ָ���ıȽ�����������ThreadSafetyList ��ĳ��Ԫ�ط�Χ������Ԫ�أ������ظ�Ԫ�ش��㿪ʼ������
        /// </summary>
        /// <param name="index">Ҫ�����ķ�Χ���㿪ʼ����ʼ����</param>
        /// <param name="count">Ҫ�����ķ�Χ�ĳ���</param>
        /// <param name="item">Ҫ��λ�Ķ��󡣶����������ͣ���ֵ����Ϊnull</param>
        /// <param name="comparer">�Ƚ�Ԫ��ʱҪʹ�õ� System.Collections.Generic.IComparerʵ�֡�- �� - Ϊnull ��ʹ��Ĭ�ϱȽ���
        /// ystem.Collections.Generic.Comparer.Default</param>
        /// <returns>����ҵ� item����Ϊ�������ThreadSafetyList�� item �Ĵ��㿪ʼ������������Ϊһ���������ø����Ǵ���
        ///     item �ĵ�һ��Ԫ�ص������İ�λ�󲹡����û�и����Ԫ�أ���ΪThreadSafetyList.Count�İ�λ�󲹡�</returns>
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            lock (this.SyncRoot)
            {
                return _list.BinarySearch(index, count, item, comparer);
            }
        }
        #endregion

        #region Clear() 
        /// <summary>
        /// ��ThreadSafetyList���Ƴ�����Ԫ��
        /// </summary>
        public void Clear()
        {
            lock (this.SyncRoot)
            {
                _list.Clear();
            }
        }
        #endregion
        
        #region Contains(T item)
        /// <summary>
        /// ȷ��ĳԪ���Ƿ���ThreadSafetyList��
        /// </summary>
        /// <param name="item">Ҫ��ThreadSafetyList�ж�λ�Ķ��󡣶����������ͣ���ֵ����Ϊnull</param>
        /// <returns>����ҵ� item����Ϊ true������Ϊ false</returns>
        public bool Contains(T item)
        {
            lock(this.SyncRoot)
            {
                return _list.Contains(item);
            }
        }
        #endregion
                
        #region CopyTo
        /// <summary>
        /// ������ThreadSafetyList���Ƶ����ݵ�һά�����У���Ŀ������Ŀ�ͷ��ʼ����
        /// </summary>
        /// <param name="array">��Ϊ��ThreadSafetyList���Ƶ�Ԫ�ص�Ŀ��λ�õ�һά System.Array��System.Array ������д��㿪ʼ������</param>
        public void CopyTo(T[] array)
        {
            lock (this.SyncRoot)
            {
                _list.CopyTo(array);
            }
        }

        /// <summary>
        /// ������ThreadSafetyList���Ƶ����ݵ�һά�����У���Ŀ�������ָ������λ�ÿ�ʼ����
        /// </summary>
        /// <param name="array">��Ϊ��ThreadSafetyList���Ƶ�Ԫ�ص�Ŀ��λ�õ�һά System.Array��System.Array ������д��㿪ʼ������</param>
        /// <param name="arrayIndex">array �д��㿪ʼ���������ڴ˴���ʼ����</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this.SyncRoot)
            {
                _list.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// ��һ����Χ��Ԫ�ش�ThreadSafetyList���Ƶ����ݵ�һά�����У���Ŀ�������ָ������λ�ÿ�ʼ����
        /// </summary>
        /// <param name="index">Դ ThreadSafetyList�и��ƿ�ʼλ�õĴ��㿪ʼ������</param>
        /// <param name="array">��Ϊ��ThreadSafetyList���Ƶ�Ԫ�ص�Ŀ��λ�õ�һά System.Array��System.Array ������д��㿪ʼ������</param>
        /// <param name="arrayIndex">array �д��㿪ʼ���������ڴ˴���ʼ����</param>
        /// <param name="count">Ҫ���Ƶ�Ԫ����</param>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            lock (this.SyncRoot)
            {
                _list.CopyTo(index, array, arrayIndex, count);
            }
        }
        #endregion

        #region IndexOf(T item) 
        /// <summary>
        /// ����ָ���Ķ��󣬲���������ThreadSafetyList�е�һ��ƥ����Ĵ��㿪ʼ��������
        /// </summary>
        /// <param name="item">Ҫ��ThreadSafetyList�ж�λ�Ķ��󡣶����������ͣ���ֵ����Ϊnull��</param>
        /// <returns>���������ThreadSafetyList���ҵ� item �ĵ�һ��ƥ�����Ϊ����Ĵ��㿪ʼ������������Ϊ-1</returns>
        public int IndexOf(T item)
        {
            lock (this.SyncRoot)
            {
                return _list.IndexOf(item);
            }
        }
        #endregion
        
        #region Insert(int index, T item) 
        /// <summary>
        /// ��Ԫ�ز���ThreadSafetyList ��ָ��������
        /// </summary>
        /// <param name="index">���㿪ʼ��������Ӧ�ڸ�λ�ò��� item</param>
        /// <param name="item">Ҫ����Ķ��󡣶����������ͣ���ֵ����Ϊnull</param>
        public void Insert(int index, T item)
        {
            lock (this.SyncRoot)
            {
                _list.Insert(index, item);
            }
        }
        #endregion
        
        #region Remove(T item) 
        /// <summary>
        ///  ��ThreadSafetyList���Ƴ��ض�����ĵ�һ��ƥ����
        /// </summary>
        /// <param name="item">Ҫ��ThreadSafetyList���Ƴ��Ķ��󡣶����������ͣ���ֵ����Ϊnull</param>
        /// <returns>����ɹ��Ƴ� item����Ϊ true������Ϊ false�����û���ҵ�item���÷���Ҳ�᷵�� false</returns>
        public bool Remove(T item)
        {
            lock (this.SyncRoot)
            {
                return _list.Remove(item);
            }
        }
        #endregion
                
        #region RemoveAt(int index) 
        /// <summary>
        /// �Ƴ�ThreadSafetyList��ָ����������Ԫ��
        /// </summary>
        /// <param name="index"> Ҫ�Ƴ���Ԫ�صĴ��㿪ʼ������</param>
        public void RemoveAt(int index)
        {
            lock (this.SyncRoot)
            {
                _list.RemoveAt(index);
            }
        }
        #endregion
        
        #region RemoveRange(int index, int count) 
        /// <summary>
        /// ��ThreadSafetyList���Ƴ�һ����Χ��Ԫ��
        /// </summary>
        /// <param name="index">Ҫ�Ƴ���Ԫ�صķ�Χ���㿪ʼ����ʼ����</param>
        /// <param name="count">Ҫ�Ƴ���Ԫ����</param>
        public void RemoveRange(int index, int count)
        {
            lock (this.SyncRoot)
            {
                _list.RemoveRange(index, count);
            }
        }
        #endregion
                
        #region ToArray() 
        /// <summary>
        /// ��ThreadSafetyList��Ԫ�ظ��Ƶ���������
        /// </summary>
        /// <returns>һ�����飬������ThreadSafetyList��Ԫ�صĸ���</returns>
        public T[] ToArray()
        {
            lock (this.SyncRoot)
            {
                return _list.ToArray();
            }
        }
        #endregion
                
        #region GetEnumerator()
        /// <summary>
        ///  ����ѭ������ThreadSafetyList ��ö����
        /// </summary>
        /// <returns>����ѭ������ThreadSafetyList ��ö����</returns>
        public IEnumerator<T> GetEnumerator()
        {
            lock (this.SyncRoot)
            {
                using (__Enumerator1 e = new __Enumerator1(this))
                {
                    while (e.MoveNext())
                    {
                        yield return e.Current;
                    }
                }
            }
        }

        /// <summary>
        /// ����һ��ѭ�����ʼ��ϵ�ö����
        /// </summary>
        /// <returns>������ѭ�����ʼ��ϵ� System.Collections.IEnumerator ����</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion


        #region AddQueue
        /// <summary>
        /// ��������Ԫ����ͬ�Ķ����е�Ԫ��,������ӵ���������
        /// </summary>
        /// <param name="q">������Ԫ����ͬ�Ķ���</param>
        /// <returns>�ɹ�Ϊtrue;����Ϊfalse</returns>
        public bool AddQueue(Queue q)
        {
            if (q == null)
            {
                throw new ArgumentNullException("������������Ϊ��");
            }

            if (q.Count == 0)
            {
                return true;
            }

            lock (this.SyncRoot)
            {
                Type qType = q.Peek().GetType();
                Type lType = typeof(T);
                if (qType != lType)
                {
                    throw new Exception(string.Format("���Ͳ�һ��! ThreadSafetyList����Ϊ{0};Queue����Ϊ{1}", lType, qType));
                }

                foreach (T t in q)
                {
                    _list.Add(t);
                }

                return true;
            }
        }
        #endregion

        #region Dequeue 
        /// <summary>
        /// �Ƴ�������λ��ThreadSafetyList��ʼ���Ķ���
        /// </summary>
        /// <returns>��ThreadSafetyList�Ŀ�ͷ�Ƴ��Ķ���</returns>
        public T Dequeue()
        {
            lock (this.SyncRoot)
            {
                T t = _list[0];
                _list.RemoveAt(0);
                return t;
            }
        }
        #endregion

        #region Enqueue 
        /// <summary>
        /// ��������ӵ�ThreadSafetyList �Ľ�β����
        /// </summary>
        /// <param name="item">Ҫ��ӵ�ThreadSafetyList�Ķ���</param>
        public void Enqueue(T item)
        {
            lock (this.SyncRoot)
            {
                this.Add(item);
            }
        }
        #endregion
        
        #region IList ��Ա
        /// <summary>
        /// ��ĳ����ӵ� System.Collections.IList �С�
        /// </summary>
        /// <param name="value">Ҫ��ӵ� System.Collections.IList �� System.Object��</param>
        /// <returns>��Ԫ�صĲ���λ��</returns>
        int IList.Add(object value)
        {
            lock(this.SyncRoot)
            {
                return ((IList)_list).Add(value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IList.Contains(object value)
        {
            lock (this.SyncRoot)
            {
                return ((IList)_list).Contains(value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        int IList.IndexOf(object value)
        {
            lock (this.SyncRoot)
            {
                return ((IList)_list).IndexOf(value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        void IList.Insert(int index, object value)
        {
            lock (this.SyncRoot)
            {
                ((IList)_list).Insert(index, value);
            }
        }
        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ System.Collections.IList �Ƿ���й̶���С��
        /// ��� System.Collections.IList ���й̶���С����Ϊ true������Ϊ false��
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void IList.Remove(object value)
        {
            lock (this.SyncRoot)
            {
                ((IList)_list).Remove(value);
            }
        }

        object IList.this[int index]
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return ((IList)_list)[index];
                }
            }
            set
            {
                lock (this.SyncRoot)
                {
                    ((IList)_list)[index] = value;
                }
            }
        }

        #endregion

        #region ICollection ��Ա
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        void ICollection.CopyTo(Array array, int index)
        {
            lock (this.SyncRoot)
            {
                ((IList)_list).CopyTo(array, index);
            }
        }
        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ�Ƿ�ͬ���� System.Collections.ICollection �ķ��ʣ��̰߳�ȫ����
        /// </summary>
        public bool IsSynchronized
        {
            get { return true; }
        }
        /// <summary>
        /// ��ȡ������ͬ�� System.Collections.ICollection ���ʵĶ���
        /// </summary>
        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        #endregion

        #region ICollection<T> ��Ա

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ�������Ƿ�Ϊֻ����
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion
    }
}

