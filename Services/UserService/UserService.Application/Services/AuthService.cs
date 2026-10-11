using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTOs.Auth;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        
      

        public AuthService(IUserRepository userRepository ,
            IPasswordHasherService passwordHasher,
            IMapper mapper,
            IRoleRepository roleRepository
            
            )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _roleRepository = roleRepository;
            
        }
        public async Task RegisterPatientAsync(RegisterPatientRequest request)
        {
            var existingEmail = await _userRepository.GetByEmailAsync(request.Email);

            if(existingEmail != null)
            {
                throw new BadRequestException("An account with this email already exists.");
            }

            var patientRole = await _roleRepository.GetRoleByName("Patient");

            if(patientRole == null)
            {
                throw new NotFoundException("Patient role is not configured in the database.");
            }

            var user = _mapper.Map<User>(request);
            user.Id = Guid.NewGuid();
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            user.IsActive = true;
            user.CreatedAt = DateTime.UtcNow;

            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = patientRole.Id
                
            });

            var patientProfile = new PatientProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                BloodGroup = null,
                EmergencyContactName = null,
                MedicalHistory = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            user.PatientProfile = patientProfile;

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

        }

       public async Task RegisterDoctorAsync(RegisterDoctorRequest request)
        {
            var existingDoctor = await _userRepository.GetByEmailAsync(request.Email);

            if(existingDoctor != null)
            {
                throw new BadRequestException("An account with this email already exists.");

            }

            var doctorRole = await _roleRepository.GetRoleByName("Doctor");

            if(doctorRole == null)
            {
                throw new NotFoundException("Doctor role is not configured in the database.");

            }

            var user = _mapper.Map<User>(request);
            user.Id = Guid.NewGuid();
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            user.IsActive = false;
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = doctorRole.Id
            });

            var doctorProfile = _mapper.Map<DoctorProfile>(request);
            doctorProfile.Id = Guid.NewGuid();
            doctorProfile.UserId = user.Id;
            doctorProfile.IsVerified = false;
            doctorProfile.CreatedAt = DateTime.UtcNow;
            doctorProfile.UpdatedAt = DateTime.UtcNow;

            user.DoctorProfile = doctorProfile;

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

        }
    }
}
