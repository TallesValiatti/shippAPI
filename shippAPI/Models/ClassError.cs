using System;
namespace shippAPI
{
    public class ClassError
    {
        #region variables

        private bool _error;
        private string _message;

        #endregion

        #region properties

        public bool error
        {
            get
            {
                return _error;
            }
        }

        public string message
        {
            get
            {
                return _message;
            }
        }

        #endregion

        #region methods

        public ClassError(bool _error, string  _message)
        {
            this._error = _error;
            this._message = _message;
        }

        #endregion
    }
}
