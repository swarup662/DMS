﻿using DMS.COMMON.Models;

namespace DMS.API.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        (bool Success, string Message, User User, string Token) Login(string userName, string password, string ipAddress);
        int Logout(int userID, string ipAddress);
        int LogHttpError(LogHttpErrorRequest request);
        int LogException(LogExceptionRequest request);
    }
}
