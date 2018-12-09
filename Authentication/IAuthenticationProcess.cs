using Authentication.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authentication
{
    public interface IAuthenticationProcess
    {
        object LoginProcess(User user);
        object RegisterProcess(User user);
    }
}
