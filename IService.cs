using System.Collections.Generic;
using CoreWCF;
using WCFService.DTOs;

namespace WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        List<UserDTO> Get();

        [OperationContract]
        UserDTO? GetUserById(int Id);

        [OperationContract]
        bool InsertUser(UserDTO User);

        [OperationContract]
        void UpdateUser(UserDTO User);

        [OperationContract]
        void DeleteUser(int Id);
    }
}
