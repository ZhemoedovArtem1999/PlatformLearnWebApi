export interface AuthentcationDto {
  email?: string | null;
  password?: string | null;
}

export interface AuthentcationResponseDto {
  token?: string | null;
  username?: string | null;
}

export interface RegisterRequestDto {
  confirmPassword?: string | null;
  /** @format date-time */
  dateBirth?: string;
  email?: string | null;
  firstName?: string | null;
  gender?: string | null;
  lastName?: string | null;
  middleName?: string | null;
  password?: string | null;
}

export interface RegisterResponse {
  message?: string | null;
  success?: boolean;
}

export interface ValidationProblemDetails {
  detail?: string | null;
  errors?: Record<string, string[]>;
  instance?: string | null;
  /** @format int32 */
  status?: number | null;
  title?: string | null;
  type?: string | null;
  [key: string]: any;
}
