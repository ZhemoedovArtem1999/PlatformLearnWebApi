import { HttpClient, RequestParams } from "./http-client";

export class TestAuthApi<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags TestAuth
   * @name Login
   * @request POST:/api/TestAuth/auth111
   * @secure
   */
  login = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/api/TestAuth/auth111`,
      method: "POST",
      secure: true,
      format: "json",
      ...params,
    });
}
