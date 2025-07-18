import {
  AuthentcationDto,
  AuthentcationResponseDto,
  RegisterRequestDto,
  RegisterResponse,
  ValidationProblemDetails,
} from "./data-contracts";
import { ContentType, HttpClient, RequestParams } from "./http-client";

export class AuthApi<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Auth
   * @name Login
   * @request POST:/api/Auth/auth
   * @secure
   */
  login = (data: AuthentcationDto, params: RequestParams = {}) =>
    this.request<AuthentcationResponseDto, ValidationProblemDetails>({
      path: `/api/Auth/auth`,
      method: "POST",
      body: data,
      secure: true,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Auth
   * @name Register
   * @request POST:/api/Auth/register
   * @secure
   */
  register = (data: RegisterRequestDto, params: RequestParams = {}) =>
    this.request<RegisterResponse, ValidationProblemDetails>({
      path: `/api/Auth/register`,
      method: "POST",
      body: data,
      secure: true,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
}
