using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.DLL;

namespace AspNetCore6WebApp.Business
{
    public interface IUserBo
    {
        int AddOrUpdate(User o);
        void Delete(int id);
        User? GetById(int id);
        IQueryable<User>? GetByName(string? name);
        List<KeyValuePair<string, string>> ValidateForm(User o);
    }

    public class UserBo : IUserBo
    {
        private readonly IUserDao _userDao;

        public UserBo(IUserDao userDao)
        {
            _userDao = userDao;
        }

        public int AddOrUpdate(User o)
        {
            if (o.UserId == 0)
            {
                o.CreatedBy = o.UpdatedBy;
                o.CreatedDt = o.UpdatedDt;
            }
            return _userDao.AddOrUpdate(o);
        }

        public void Delete(int id)
        {
            _userDao.Delete(id);
        }

        public User? GetById(int id)
        {
            return _userDao.GetById(id);
        }

        public IQueryable<User>? GetByName(string? name)
        {
            return _userDao.GetByName(name);
        }

        //private List<KeyValuePair<string, string>> CheckMaxLength(string? field, int maxLength, string errorKey, string errorValueFormat, List<KeyValuePair<string, string>> errorList)
        //{
        //    if (string.IsNullOrEmpty(field) == false && field.Length > maxLength) errorList.Add(new KeyValuePair<string, string>(errorKey, string.Format(errorValueFormat, maxLength)));
        //    return errorList;
        //}

        public List<KeyValuePair<string, string>> ValidateForm(User o)
        {
            List<KeyValuePair<string, string>> errorList = new();
            if (string.IsNullOrEmpty(o.LoginName)) errorList.Add(new KeyValuePair<string, string>("User.LoginName", "Login Name cannot be empty."));
            else
            {
                if (o.LoginName.Length > Param.MaxLength.User.LoginName) errorList.Add(new KeyValuePair<string, string>("User.LoginName", string.Format("Login Name cannot be more than {0} characters.", Param.MaxLength.User.LoginName)));
                if ((o.UserId > 0 && _userDao.LoginNameExists(o.LoginName, o.UserId))
                || (o.UserId == 0 && _userDao.LoginNameExists(o.LoginName)))
                    errorList.Add(new KeyValuePair<string, string>(string.Empty, "Login Name already exists."));
            }
            if (string.IsNullOrEmpty(o.DisplayName) == false && o.DisplayName.Length > Param.MaxLength.User.DisplayName) errorList.Add(new KeyValuePair<string, string>("User.DisplayName", string.Format("Display Name cannot be more than {0} characters.", Param.MaxLength.User.DisplayName)));
            //errorList = CheckMaxLength(o.DisplayName, Param.MaxLength.User.DisplayName, "User.DisplayName", "Display Name cannot be more than {0} characters.", errorList);
            if (string.IsNullOrEmpty(o.Password) == false && o.Password.Length > Param.MaxLength.User.Password) errorList.Add(new KeyValuePair<string, string>("User.Password", string.Format("Password cannot be more than {0} characters.", Param.MaxLength.User.Password))); 
            if (o.Birthday != null && o.Birthday > DateTime.Now) errorList.Add(new KeyValuePair<string, string>("User.Birthday", "Birthday cannot be larger than today."));
            if (string.IsNullOrEmpty(o.Description) == false && o.Description.Length > Param.MaxLength.User.Description) errorList.Add(new KeyValuePair<string, string>("User.Description", string.Format("Description cannot be more than {0} characters.", Param.MaxLength.User.Description)));
            return errorList;
        }
    }
}
