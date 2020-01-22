using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rewind.One.AuthServer
{
    public class Constants
    {

        #region Scope Related Constants
        public const string SCOPE_DEV_ENV_NAME = "dev_environment";
        public const string SCOPE_DEV_ENV_DESC = "Developer Enviroment Scope List";
        #endregion

        #region Claim Related Constants
        public const string CLAIM_DEV_PLATFORM_NAME = "dev_platform";
        public const string CLAIM_DEV_LANG_NAME = "dev_prog_lang";
        #endregion

        #region API Related Constants
        public const string API_CRYPTO_NAME = "crypto_api";
        public const string API_CRYPTO_DESC = "Crypto Api";
        public const string API_CRYPTO_SECRET = "cryptoapisecret";

        #endregion
    }
}
