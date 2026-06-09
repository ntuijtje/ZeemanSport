using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Location;

namespace ZeemanSport.Core.Location
{
    public class Seat
    {
        #region Fields

        private int _locationId;
        private int _rowIndex;
        private int _columnIndex;
        private bool _isReserved;
        private bool _isSelected;

        #endregion

        #region Constructor

        public Seat()
        {
        }

        #endregion

        #region Properties

        public int LocationId
        {
            get => _locationId;
            set => _locationId = value;
        }

        public int RowIndex
        {
            get => _rowIndex;
            set => _rowIndex = value;
        }

        public int ColumnIndex
        {
            get => _columnIndex;
            set => _columnIndex = value;
        }

        public bool IsReserved
        {
            get => _isReserved;
            set => _isReserved = value;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => _isSelected = value;
        }

        #endregion

    }
}
