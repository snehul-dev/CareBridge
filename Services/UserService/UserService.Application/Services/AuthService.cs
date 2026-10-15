
using AutoMapper;
using UserService.Application.DTOs.Auth;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IMapper _mapper;
    private readonly IRoleRepository _roleRepository;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasherService passwordHasher,
        IMapper mapper,
        IJwtService jwtService,
        IRefreshTokenRepository refreshTokenRepository,
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
        _roleRepository = roleRepository;
    }

    // PATIENT REGISTRATION

    public async Task RegisterPatientAsync(RegisterPatientRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = request.Email.Trim().ToLowerInvariant();
        var phoneNumber = request.PhoneNumber.Trim();
        var now = DateTime.UtcNow;

        var patientRole = await _roleRepository.GetRoleByName("Patient")
            ?? throw new NotFoundException(
                "Patient role is not configured in the database.");

        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser != null)
        {
            var roles = GetRoleNames(existingUser);

            if (roles.Contains("Admin") || roles.Contains("Lab"))
            {
                throw new BadRequestException(
                    "This email cannot be used for Patient registration.");
            }

            if (roles.Contains("Patient"))
            {
                throw new BadRequestException(
                    "A Patient account already exists for this email.");
            }

            if (!roles.Contains("Doctor"))
            {
                throw new BadRequestException(
                    "This email cannot be reused for Patient registration.");
            }

            ValidateMatchingPassword(
                request.Password,
                existingUser.PasswordHash);

            ValidateMatchingPhoneNumber(
                phoneNumber,
                existingUser.PhoneNumber);

            if (existingUser.PatientProfile != null)
            {
                throw new BadRequestException(
                    "A Patient profile already exists for this account.");
            }

            var patientProfile = new PatientProfile
            {
                Id = Guid.NewGuid(),
                UserId = existingUser.Id,
                BloodGroup = null,
                EmergencyContactName = null,
                MedicalHistory = null,
                CreatedAt = now,
                UpdatedAt = null
            };

        
            await _userRepository.AddPatientProfileAsync(patientProfile);

            existingUser.UserRoles.Add(new UserRole
            {
                UserId = existingUser.Id,
                RoleId = patientRole.Id
            });

            existingUser.UpdatedAt = now;

            await _userRepository.SaveChangesAsync();
            return;
        }

        // Create a new User when this email does not exist.
        var user = _mapper.Map<User>(request);

        user.Id = Guid.NewGuid();
        user.Email = email;
        user.PhoneNumber = phoneNumber;
        user.PasswordHash = _passwordHasher.HashPassword(request.Password);
        user.IsActive = true;
        user.CreatedAt = now;
        user.UpdatedAt = null;

        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = patientRole.Id
        });

        user.PatientProfile = new PatientProfile
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            BloodGroup = null,
            EmergencyContactName = null,
            MedicalHistory = null,
            CreatedAt = now,
            UpdatedAt = null
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    // DOCTOR REGISTRATION

    public async Task RegisterDoctorAsync(RegisterDoctorRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = request.Email.Trim().ToLowerInvariant();
        var phoneNumber = request.PhoneNumber.Trim();
        var now = DateTime.UtcNow;

        var doctorRole = await _roleRepository.GetRoleByName("Doctor")
            ?? throw new NotFoundException(
                "Doctor role is not configured in the database.");

        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser != null)
        {
            var roles = GetRoleNames(existingUser);

            if (roles.Contains("Admin") || roles.Contains("Lab"))
            {
                throw new BadRequestException(
                    "This email cannot be used for Doctor registration.");
            }

            if (roles.Contains("Doctor"))
            {
                throw new BadRequestException(
                    "A Doctor account already exists for this email.");
            }

            if (!roles.Contains("Patient"))
            {
                throw new BadRequestException(
                    "This email cannot be reused for Doctor registration.");
            }

            ValidateMatchingPassword(
                request.Password,
                existingUser.PasswordHash);

            ValidateMatchingPhoneNumber(
                phoneNumber,
                existingUser.PhoneNumber);

            if (existingUser.DoctorProfile != null)
            {
                throw new BadRequestException(
                    "A Doctor profile already exists for this account.");
            }

            var doctorProfile = _mapper.Map<DoctorProfile>(request);

            doctorProfile.Id = Guid.NewGuid();
            doctorProfile.UserId = existingUser.Id;
            doctorProfile.IsVerified = false;
            doctorProfile.CreatedAt = now;
            doctorProfile.UpdatedAt = now;

         
            await _userRepository.AddDoctorProfileAsync(doctorProfile);

            existingUser.UserRoles.Add(new UserRole
            {
                UserId = existingUser.Id,
                RoleId = doctorRole.Id
            });

            existingUser.UpdatedAt = now;

            await _userRepository.SaveChangesAsync();
            return;
        }

        // No existing User create a new Doctor account.
        var user = _mapper.Map<User>(request);

        user.Id = Guid.NewGuid();
        user.Email = email;
        user.PhoneNumber = phoneNumber;
        user.PasswordHash = _passwordHasher.HashPassword(request.Password);
        user.IsActive = true;
        user.CreatedAt = now;
        user.UpdatedAt = now;

        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = doctorRole.Id
        });

        var profile = _mapper.Map<DoctorProfile>(request);

        profile.Id = Guid.NewGuid();
        profile.UserId = user.Id;
        profile.IsVerified = false;
        profile.CreatedAt = now;
        profile.UpdatedAt = now;

        user.DoctorProfile = profile;

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    // LAB REGISTRATION

    public async Task RegisterLabAsync(RegisterLabRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = request.Email.Trim().ToLowerInvariant();
        var phoneNumber = request.PhoneNumber.Trim();
        var now = DateTime.UtcNow;

        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser != null)
        {
            throw new BadRequestException(
                "This email is already in use. Lab registration requires a unique email.");
        }

        var labRole = await _roleRepository.GetRoleByName("Lab")
            ?? throw new NotFoundException(
                "Lab role is not configured in the database.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PhoneNumber = phoneNumber,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = labRole.Id
        });

        var labProfile = _mapper.Map<LabProfile>(request);

        labProfile.Id = Guid.NewGuid();
        labProfile.UserId = user.Id;
        labProfile.IsVerified = false;
        labProfile.CreatedAt = now;
        labProfile.UpdatedAt = now;

        user.LabProfile = labProfile;

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    private static HashSet<string> GetRoleNames(User user)
    {
        return user.UserRoles
            .Where(ur => ur.Role != null)
            .Select(ur => ur.Role.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private void ValidateMatchingPassword(
        string requestPassword,
        string existingPasswordHash)
    {
        if (!_passwordHasher.VerifyPassword(
                requestPassword,
                existingPasswordHash))
        {
            throw new BadRequestException(
                "The password does not match the existing account.");
        }
    }

    private static void ValidateMatchingPhoneNumber(
        string requestPhoneNumber,
        string existingPhoneNumber)
    {
        if (!string.Equals(
                requestPhoneNumber,
                existingPhoneNumber.Trim(),
                StringComparison.Ordinal))
        {
            throw new BadRequestException(
                "The phone number does not match the existing account.");
        }
    }

    // LOGIN

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new BadRequestException(
                "Email and password are required.");
        }

        var allowedRoles = new[]
        {
            "Patient",
            "Doctor",
            "Lab",
            "Admin"
        };

        var role = allowedRoles.FirstOrDefault(r =>
            r.Equals(
                request.Role?.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (role is null)
        {
            throw new BadRequestException(
                "Please select a valid role.");
        }

        var email = request.Email.Trim();

        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null ||
            !_passwordHasher.VerifyPassword(
                request.Password,
                user.PasswordHash))
        {
            throw new BadRequestException(
                "Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new BadRequestException(
                "Your account is inactive.");
        }

        var hasSelectedRole = user.UserRoles.Any(ur =>
            ur.Role != null &&
            ur.Role.Name.Equals(
                role,
                StringComparison.OrdinalIgnoreCase));

        if (!hasSelectedRole)
        {
            throw new BadRequestException(
                "The selected role is not assigned to your account.");
        }

        // A Doctor must be approved before Doctor login.
        if (role == "Doctor" &&
            (user.DoctorProfile is null ||
             !user.DoctorProfile.IsVerified))
        {
            throw new BadRequestException(
                "Your doctor account is awaiting verification.");
        }

        // A Lab must be approved before Lab login.
        if (role == "Lab" &&
            (user.LabProfile is null ||
             !user.LabProfile.IsVerified))
        {
            throw new BadRequestException(
                "Your lab account is awaiting verification.");
        }

        var (accessToken, expiresAt) = _jwtService.GenerateAccessToken(
                            user.Id,
                           user.Email,
                           role);

        var rawRefreshToken = RefreshTokenHelper.Generate();
        var now = DateTime.UtcNow;

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = RefreshTokenHelper.Hash(rawRefreshToken),
            Role = role,
            CreatedAt = now,
            ExpiresAt = now.AddDays(7),
            RevokedAt = null
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Role = role,
            AccessToken = accessToken,
            AccessTokenExpiresAt = expiresAt,
            RefreshToken = rawRefreshToken
        };
    }


    public async Task<LoginResponse> RefreshAsync(
        RefreshTokenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new BadRequestException(
                "Refresh token is required.");
        }

        var tokenHash = RefreshTokenHelper.Hash(request.RefreshToken);

        var storedToken = await _refreshTokenRepository.GetByHashAsync(tokenHash);

        if (storedToken is null ||
            storedToken.RevokedAt.HasValue ||
            storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid, expired, or revoked refresh token.");
        }

        var user = await _userRepository.GetByIdAsync(storedToken.UserId);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedException("User is inactive or does not exist.");
        }

      
        var role = storedToken.Role;

        var hasRole = user.UserRoles.Any(ur =>
            ur.Role != null &&
            ur.Role.Name.Equals(role,StringComparison.OrdinalIgnoreCase));

        if (!hasRole)
        {
            throw new UnauthorizedException(
                "This role is no longer assigned to the user.");
        }

 
        if (role.Equals("Doctor",
                StringComparison.OrdinalIgnoreCase) &&
            (user.DoctorProfile is null ||
             !user.DoctorProfile.IsVerified))
        {
            throw new UnauthorizedException(
                "Doctor account is not verified.");
        }

        if (role.Equals("Lab",
                StringComparison.OrdinalIgnoreCase) &&
            (user.LabProfile is null ||
             !user.LabProfile.IsVerified))
        {
            throw new UnauthorizedException(
                "Lab account is not verified.");
        }

        var now = DateTime.UtcNow;

        var (accessToken, accessTokenExpiresAt) =
            _jwtService.GenerateAccessToken(
                user.Id,
                user.Email,
                role);

        var newRawRefreshToken = RefreshTokenHelper.Generate();

        storedToken.RevokedAt = now;

        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = RefreshTokenHelper.Hash(
                newRawRefreshToken),
            Role = role,
            CreatedAt = now,
            ExpiresAt = now.AddDays(7),
            RevokedAt = null
        };

        await _refreshTokenRepository.AddAsync(newRefreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Role = role,
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshToken = newRawRefreshToken
        };
    }

}
