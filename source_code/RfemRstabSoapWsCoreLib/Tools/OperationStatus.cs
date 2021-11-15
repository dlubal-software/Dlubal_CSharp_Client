namespace Dlubal.WS.RfemRstabSoapWsCoreLib.Tools
{
    public enum OperationStatusType
    {
        OK = 0,
        WARNING = 1,
        ERROR = 2,
    }

    public class OperationStatus
    {
        private OperationStatusType type = OperationStatusType.OK;
        private string description = string.Empty;

        public OperationStatusType Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
                if (type == OperationStatusType.OK)
                {
                    description = string.Empty;
                }
            }
        }

        public string ErrorDescription
        {
            get
            {
                return type == OperationStatusType.ERROR ? description : string.Empty;
            }

            set
            {
                type = OperationStatusType.ERROR;
                description = value;
            }
        }

        public string WarningDescription
        {
            get
            {
                return type == OperationStatusType.WARNING ? description : string.Empty;
            }

            set
            {
                type = OperationStatusType.WARNING;
                description = value;
            }
        }

        public OperationStatus()
        {
        }

        public OperationStatus(OperationStatusType type, string description)
        {
            this.type = type;
            this.description = description;
        }

        public override string ToString() => string.Format($"{type}: {description}");
    }
}
