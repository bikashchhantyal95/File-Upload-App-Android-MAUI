using System;
namespace CloudAssignmentAC4.Models
{
	public class ImageModel
	{
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
        public string DownloadUrl { get; set; }
    }
}

