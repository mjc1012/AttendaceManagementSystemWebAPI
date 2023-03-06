﻿namespace AttendaceManagementSystemWebAPI.Helper
{
    public class ResponseApi<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Value { get; set; }
    }
}
