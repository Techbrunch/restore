using System;

namespace Restore
{
    public class SynchronizationAction<T> : ISynchronizationAction<T>
    {
        private readonly Func<IDataEndpoint<T>, T, bool> _applies;
        private readonly Action<IDataEndpoint<T>, T> _executeAction;
        private readonly IDataEndpoint<T> _dataEndpoint;
        private readonly string _name;
        private T _applicant;

        public SynchronizationAction(Func<IDataEndpoint<T>, T, bool> applies, Action<IDataEndpoint<T>, T> executeAction, IDataEndpoint<T> dataEndpoint, string name = "Unnamed")
        {
            _applies = applies;
            _executeAction = executeAction;
            _dataEndpoint = dataEndpoint;
            _name = name;
        }

        public T Applicant
        {
            get { return _applicant; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool AppliesTo(T resource)
        {
            var applies = _applies(_dataEndpoint, resource);
            if (applies)
            {
                _applicant = resource;
            }
            return applies;
        }

        public void Execute()
        {
            _executeAction(_dataEndpoint, _applicant);
        }

        public override string ToString()
        {
            string result = _name;
            if (Applicant != null)
            {
                var identityResolver = _dataEndpoint.IdentityResolver(Applicant);
                result = string.Format("{0} Will be applied to resource of Type {1} with id {2}", _name, typeof (T).Name,
                    identityResolver);
            }
            return result;
        }
    }
}