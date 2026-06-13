using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Subscription
{
    public class SubscriptionPlan
    {
        #region Fields

        private int _id;
        private string? _name;
        private AccessTier _accessTier;
        private BillingInterval _billingInterval;
        private decimal _price;
        private bool _isActive;

        #endregion

        #region Constructor

        public SubscriptionPlan()
        {
            _id = 0;
            _isActive = true;
        }

        #endregion

        #region Properties

        public int Id
        {
            get => _id;
        }

        public string? Name
        {
            get => _name;
            set => _name = value;
        }

        public AccessTier AccessTier
        {
            get => _accessTier;
            set => _accessTier = value;
        }

        public BillingInterval BillingInterval
        {
            get => _billingInterval;
            set => _billingInterval = value;
        }

        public decimal Price
        {
            get => _price;
            set => _price = value;
        }

        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        #endregion

        #region Methods

        public void SetId(int id)
        {
            if (_id == 0)
                _id = id;
        }

        #endregion
    }
}
