/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�Microunion Foundation Component Library
//
// �� �� �ˣ�zhh_217
// �������ڣ�08/31/2011 08:53:27 
// ��    �䣺zhh_217@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾ 2009-2015 ��������Ȩ��
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;

namespace BJMT.RsspII4net.Utilities
{
    /// <summary>
    /// ��Ʒ�����¼�������
    /// </summary>
    /// <typeparam name="TProduct"></typeparam>
    class ProductCreatedEventArgs<TProduct> : EventArgs
    {
        #region "Filed"
        private List<TProduct> _products = new List<TProduct>();
        #endregion

        #region "Constructor"
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="product">��Ʒ����</param>
        public ProductCreatedEventArgs(TProduct product)
        {
            this._products.Add(product);
        }
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="products">��Ʒ��</param>
        public ProductCreatedEventArgs(IList<TProduct> products)
        {
            this._products.AddRange(products);
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// ��ȡ��Ʒ����
        /// </summary>
        public IList<TProduct> Products
        {
            get { return _products; }            
        }
        #endregion

        #region "Override methods"
        #endregion

        #region "Private methods"
        #endregion
    }

}
