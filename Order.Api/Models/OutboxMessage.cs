using System.ComponentModel.DataAnnotations;

namespace Orders.Api.Models;

public class OutboxMessage {
	[Key]
	public Guid? Id { get; set; }
	public string Type { get; set; }
	public string Payload { get; set; }
	public DateTime CreatedAtUTC { get; set; }
	public DateTime? ProcessedAtUTC { get; set; }


	public OutboxMessage() {
	}

	public OutboxMessage(Guid? id, string type, string payload, DateTime createdAtUtc, DateTime? processedAtUtc) {
		Id = id;
		Type = type;
		Payload = payload;
		CreatedAtUTC = createdAtUtc;
		ProcessedAtUTC = processedAtUtc;
	}
}