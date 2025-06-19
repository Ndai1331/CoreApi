using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace CoreAPI.Models.BaseResponse
{
    public class RequestHttpResponse<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; } 
        [JsonPropertyName("meta")]
        public Meta? Meta { get; set; } 
        [JsonPropertyName("errors")]
        public List<ErrorResponse> Errors { get; set; } = new List<ErrorResponse>();
        public bool IsSuccess
        {
            get { return Errors.Count == 0; }
        }

        public string Message
        {
            get { return Errors.Count > 0 ? Errors[0].Message : string.Empty; }
        }
        public HttpStatusCode  StatusCode
        {
            set;
            get;
        }
    }

    // Directus-style response for array data
    public class DirectusResponse<T>
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = new List<T>();
        
        [JsonPropertyName("meta")]
        public DirectusMeta? Meta { get; set; }
        
        [JsonPropertyName("errors")]
        public List<ErrorResponse> Errors { get; set; } = new List<ErrorResponse>();
        
        public bool IsSuccess
        {
            get { return Errors.Count == 0; }
        }

        public string Message
        {
            get { return Errors.Count > 0 ? Errors[0].Message : string.Empty; }
        }
        
        public HttpStatusCode StatusCode { get; set; }
    }

    // Directus-style response for single item
    public class DirectusSingleResponse<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }
        
        [JsonPropertyName("meta")]
        public DirectusMeta? Meta { get; set; }
        
        [JsonPropertyName("errors")]
        public List<ErrorResponse> Errors { get; set; } = new List<ErrorResponse>();
        
        public bool IsSuccess
        {
            get { return Errors.Count == 0; }
        }

        public string Message
        {
            get { return Errors.Count > 0 ? Errors[0].Message : string.Empty; }
        }
        
        public HttpStatusCode StatusCode { get; set; }
    }

    public class ErrorResponse
    { 
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        
        [JsonPropertyName("extensions")]
        public ExtensionsResponse Extensions { get; set; } = new();
    }
    
    public class ExtensionsResponse
    { 
        [JsonPropertyName("code")]
        public string code { get; set; } = string.Empty;
        [JsonPropertyName("reason")]
        public string reason { get; set; } = string.Empty;
    }      

    public class Meta
    {
        [JsonPropertyName("total_count")]
        public int? total_count { get; set; }
        [JsonPropertyName("filter_count")]
        public int? filter_count { get; set; }
        [JsonPropertyName("page")]
        public int? page { get; set; }
        [JsonPropertyName("page_count")]
        public int? page_count { get; set; }
        [JsonPropertyName("limit")]
        public int? limit { get; set; }
        [JsonPropertyName("offset")]
        public int? offset { get; set; }
        [JsonPropertyName("fields")]
        public string? fields { get; set; }
    }

    // Directus-style metadata
    public class DirectusMeta
    {
        [JsonPropertyName("total_count")]
        public int? total_count { get; set; }
        
        [JsonPropertyName("filter_count")]
        public int? filter_count { get; set; }
        
        [JsonPropertyName("page")]
        public int? page { get; set; }
        
        [JsonPropertyName("page_count")]
        public int? page_count { get; set; }
        
        [JsonPropertyName("limit")]
        public int? limit { get; set; }
        
        [JsonPropertyName("offset")]
        public int? offset { get; set; }
        
        [JsonPropertyName("fields")]
        public string? fields { get; set; }
        
        [JsonPropertyName("sort")]
        public List<string>? sort { get; set; }
        
        [JsonPropertyName("filter")]
        public object? filter { get; set; }
    }
} 