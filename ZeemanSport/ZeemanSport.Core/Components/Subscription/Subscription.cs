using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Core.Subscription
{
    public class Subscription
    {
        #region Fields

        private int _id;
        private int _userId;
        private int _planId;
        private DateTime _startDate;
        private DateTime _endDate;
        private SubscriptionStatus _status;

        #endregion

        #region Constructor

        public Subscription()
        {
            _id = 0;
            _status = SubscriptionStatus.Pending;
        }

        #endregion

        #region Properties

        public int Id
        {
            get => _id;
        }

        public int UserId
        {
            get => _userId;
            set => _userId = value;
        }

        public int PlanId
        {
            get => _planId;
            set => _planId = value;
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = value;
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => _endDate = value;
        }

        public SubscriptionStatus Status
        {
            get => _status;
            set => _status = value;
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
