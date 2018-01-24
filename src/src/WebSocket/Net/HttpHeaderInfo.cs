

using System;

namespace WebSocketSharp.Net
{
  internal class HttpHeaderInfo
  {
    #region Private Fields

    private string         _name;
    private HttpHeaderType _type;

    #endregion

    #region Internal Constructors

    internal HttpHeaderInfo (string name, HttpHeaderType type)
    {
      _name = name;
      _type = type;
    }

    #endregion

    #region Internal Properties

    internal bool IsMultiValueInRequest {
      get {
        return (_type & HttpHeaderType.MultiValueInRequest) == HttpHeaderType.MultiValueInRequest;
      }
    }

    internal bool IsMultiValueInResponse {
      get {
        return (_type & HttpHeaderType.MultiValueInResponse) == HttpHeaderType.MultiValueInResponse;
      }
    }

    #endregion

    #region Public Properties

    public bool IsRequest {
      get {
        return (_type & HttpHeaderType.Request) == HttpHeaderType.Request;
      }
    }

    public bool IsResponse {
      get {
        return (_type & HttpHeaderType.Response) == HttpHeaderType.Response;
      }
    }

    public string Name {
      get {
        return _name;
      }
    }

    public HttpHeaderType Type {
      get {
        return _type;
      }
    }

    #endregion

    #region Public Methods

    public bool IsMultiValue (bool response)
    {
      return (_type & HttpHeaderType.MultiValue) == HttpHeaderType.MultiValue
             ? (response ? IsResponse : IsRequest)
             : (response ? IsMultiValueInResponse : IsMultiValueInRequest);
    }

    public bool IsRestricted (bool response)
    {
      return (_type & HttpHeaderType.Restricted) == HttpHeaderType.Restricted
             ? (response ? IsResponse : IsRequest)
             : false;
    }

    #endregion
  }
}
