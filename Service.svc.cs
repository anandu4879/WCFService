using System;
using System.Collections.Generic;
using System.Linq;
using WCFService.DAL.Entities;
using WCFService.DTOs;
using CoreWCF;
using Microsoft.Extensions.Logging;

namespace WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service.svc or Service.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Service : IService
    {
        private readonly ILogger<Service> _logger;
        private readonly DBContext _dbContext;

        public Service(ILogger<Service> logger, DBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        
        public List<UserDTO> Get()
        {
            try
            {
                _logger.LogInformation("Attempting to get all users");
                var users = _dbContext.User.Select(
                    s => new UserDTO
                    {
                        Id = s.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Username = s.Username,
                        Password = s.Password,
                        EnrollmentDate = s.EnrollmentDate
                    }
                ).ToList();
                _logger.LogInformation($"Retrieved {users.Count} users");
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users: {Message}", ex.Message);
                throw new FaultException<string>($"Database error: {ex.Message}", new FaultReason(ex.Message));
            }
        }

        public UserDTO? GetUserById(int Id)
        {
            return _dbContext.User.Select(
                    s => new UserDTO
                    {
                        Id = s.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Username = s.Username,
                        Password = s.Password,
                        EnrollmentDate = s.EnrollmentDate
                    })
                .FirstOrDefault(s => s.Id == Id);
        }
        
        public bool InsertUser(UserDTO User)
        {
            var entity = new User()
            {
                FirstName = User.FirstName,
                LastName = User.LastName,
                Username = User.Username,
                Password = User.Password,
                EnrollmentDate = User.EnrollmentDate
            };

            _dbContext.User.Add(entity);
            _dbContext.SaveChangesAsync();

            return true;
        }

        public void UpdateUser(UserDTO User)
        {
            var entity = _dbContext.User.FirstOrDefault(s => s.Id == User.Id);
            if (entity == null) return;

            entity.FirstName = User.FirstName;
            entity.LastName = User.LastName;
            entity.Username = User.Username;
            entity.Password = User.Password;
            entity.EnrollmentDate = User.EnrollmentDate;

            _dbContext.SaveChangesAsync();
        }

        public void DeleteUser(int Id)
        {
            var entity = new User()
            {
                Id = Id
            };

            _dbContext.User.Attach(entity);
            _dbContext.User.Remove(entity);
            _dbContext.SaveChangesAsync();
        }
    }
}