namespace OrderManagement.Domain;

public sealed class OrderConflictException(string message) : Exception(message);
