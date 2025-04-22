using Weekly.Exceptions;

namespace Weekly.Services
{
    public partial class WeeklyService
    {
        public void AddToken(string tokenName, string userName, string token)
        {
            try
            {
                _tokenManager.PutToken(tokenName, userName, token);
            }
            catch (Exception ex)
            {
                throw new InternalException(ex);
            }
        }

        public void RemoveToken(string tokenName)
        {
            try
            {
                _tokenManager.DeleteToken(tokenName);
            }
            catch (Exception ex)
            {
                throw new InternalException(ex);
            }
        }

        public void ClearTokens()
        {
            try
            {
                _tokenManager.ClearTokens();
            }
            catch (Exception ex)
            {
                throw new InternalException(ex);
            }
        }
    }
}
