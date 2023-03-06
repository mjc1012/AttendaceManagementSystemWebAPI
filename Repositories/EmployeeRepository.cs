﻿using AttendaceManagementSystemWebAPI.Data;
using AttendaceManagementSystemWebAPI.Interfaces;
using AttendaceManagementSystemWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Validations;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AttendaceManagementSystemWebAPI.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DataContext _context;
        public EmployeeRepository(DataContext context)
        { 
            _context = context;
        }

        public async Task<List<Employee>> GetEmployees()
        {
            try
            {
                return await _context.Employees.OrderBy(p => p.Id).Include(p => p.AttendanceLogs).Include(p => p.EmployeeRole).ToListAsync();
            }
            catch(Exception )
            {
                throw ;
            }
        }

        public async Task<Employee> GetEmployee(int id)
        {
            try
            {
                return await _context.Employees.Where(p => p.Id == id).Include(p => p.EmployeeRole).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw ;
            }
        }

        public async Task<Employee> GetEmployee(string employeeIdNumber)
        {
            try
            {
                return await _context.Employees.Where(p => p.EmployeeIdNumber.Trim() == employeeIdNumber.Trim()).Include(p => p.EmployeeRole).FirstOrDefaultAsync();
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public async Task<Employee> GetEmployeeByEmail(string email)
        {
            try
            {
                return await _context.Employees.Where(p => p.EmailAddress.Trim() == email.Trim()).Include(p => p.EmployeeRole).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<bool> EmployeeExists(string employeeIdNumber, string password)
        {
            try
            {
                return await _context.Employees.AnyAsync(p => p.EmployeeIdNumber == employeeIdNumber.Trim() && p.Password == password.Trim());
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public async Task<bool> EmployeeIdNumberExists(string employeeIdNumber)
        {
            try
            {
                return await _context.Employees.AnyAsync(p => p.EmployeeIdNumber == employeeIdNumber.Trim());
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public async Task<bool> EmailAddressExists(string emailAddress)
        {
            {
                try
                {
                    return await _context.Employees.AnyAsync(p => p.EmailAddress == emailAddress.Trim());
                }
                catch (Exception )
                {
                    throw ;
                }
            }
        }


        public async Task<Employee> CreateEmployee(Employee employee)
        {
            try
            {
                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();
                return employee;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DetachEmployee(Employee employee)
        {
            try
            {
                _context.Entry(employee).State = EntityState.Detached;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Employee> UpdateEmployee(Employee employee)
        {
            try
            {
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
                return employee;
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public async Task<bool> DeleteEmployee(Employee employee)
        {
            try
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception )
            {
                throw ;
            }
        }
    }
}