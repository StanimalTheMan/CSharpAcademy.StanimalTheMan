﻿using ConsoleTableExt;
using Newtonsoft.Json;
using ShiftsLoggerWebAPI.Models;
using System.Net.Http.Json;

namespace ShiftsLoggerConsoleUI;

internal static class DataAccess
{
	internal static async Task CreateShiftAsync()
	{
		// maybe separate out logic to get all user input later, but this is part of creating a shift so it's here for now
		Console.WriteLine("Enter type of shift (e.g. night, mid, morning)");
		string shiftType = Console.ReadLine();

		Console.WriteLine("Enter Start Time of shift: ");
		var startTimeInfo = Utility.GetDateTimeInput();

		Console.WriteLine("Enter End Time of shift: ");
		var endTimeInfo = Utility.GetDateTimeInput();
		while (endTimeInfo.dateTime < startTimeInfo.dateTime)
		{
			Console.WriteLine("End time has to be after start time.  Enter End Time of shift:");
			endTimeInfo = Utility.GetDateTimeInput();
		}

		Shift shift = null;
		TimeSpan duration = Utility.CalculateDuration(endTimeInfo.dateTime, startTimeInfo.dateTime);
		if (shiftType != "")
		{
			shift = new Shift()
			{
				Type = shiftType,
				StartTime = startTimeInfo.dateTime,
				EndTime = endTimeInfo.dateTime,
				Duration = duration
			};
		}
		else
		{
			shift = new Shift()
			{
				StartTime = startTimeInfo.dateTime,
				EndTime = endTimeInfo.dateTime,
				Duration = duration
			};
		}


		using (var httpClient = new HttpClient())
		{
			try
			{
				HttpResponseMessage response = await httpClient.PostAsJsonAsync("https://localhost:7204/api/shifts/", shift);

				if (response.IsSuccessStatusCode)
				{
					string responseBody = await response.Content.ReadAsStringAsync();
					Console.WriteLine($"Scuccessfully created shift:\n {responseBody}");
				}
				else
				{
					Console.WriteLine($"Failed to post data. Status code: {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Server / API may be down or some other unexpected issue is going on: {ex.Message}");
				Console.ReadLine();
			}
		}
		Console.WriteLine("Press any key to return to main menu");
		Console.ReadLine();
	}

	internal static async Task DeleteShiftAsync(int id)
	{
		using (var httpClient = new HttpClient())
		{
			try
			{
				HttpResponseMessage response = await httpClient.DeleteAsync($"https://localhost:7204/api/shifts/{id}");
				if (response.IsSuccessStatusCode)
				{
					Console.WriteLine("Successfully deleted shift");
				}
				else
				{
					Console.WriteLine("Failed to delete shift");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Server / API may be down or some other unexpected issue is going on: {ex.Message}");
			}
		}

		Console.WriteLine("Press any key to return to main menu");
		Console.ReadLine();
	}

	internal static async Task GetAllShiftsAsync()
	{
		using (var httpClient = new HttpClient())
		{
			try
			{
				HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7204/api/shifts/");

				if (response.IsSuccessStatusCode)
				{
					string responseBody = await response.Content.ReadAsStringAsync();
					Console.WriteLine("Response from API:");

					var shifts = JsonConvert.DeserializeObject<List<Shift>>(responseBody);

					if (shifts != null && shifts.Count > 0)
					{
						// Visualize response in table format
						ConsoleTableBuilder
							.From(shifts)
							.WithFormat(ConsoleTableBuilderFormat.MarkDown)
							.ExportAndWriteLine();
					}
					else
					{
						Console.WriteLine("No shifts found in the response.");
					}
				}
				else
				{
					Console.WriteLine($"Failed to fetch data. Status code: {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Server / API may be down or some other unexpected issue is going on: {ex.Message}");
			}
		}
		Console.WriteLine("Press any key to return to main menu");
		Console.ReadLine();
	}

	internal static async Task GetShiftByIdAsync(int id)
	{
		using (var httpClient = new HttpClient())
		{
			try
			{
				HttpResponseMessage response = await httpClient.GetAsync($"https://localhost:7204/api/shifts/{id}");

				if (response.IsSuccessStatusCode)
				{
					string responseBody = await response.Content.ReadAsStringAsync();
					Console.WriteLine("Response from API:");

					var shift = JsonConvert.DeserializeObject<Shift>(responseBody);

					var table = ConsoleTableBuilder
						.From(new List<Shift> { shift })
						.Export();
					Console.WriteLine(table);
				}
				else
				{
					Console.WriteLine($"Failed to fetch data. Status code: {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Server / API may be down or some other unexpected issue is going on: {ex.Message}");
			}
		}
		Console.WriteLine("Press any key to return to main menu");
		Console.ReadLine();
	}

	internal static async Task UpdateShiftAsync(int id)
	{
		// do I fetch the Task with id?
		// maybe separate out logic to get all user input later, but this is part of creating a shift so it's here for now
		Console.WriteLine("Enter type of shift (e.g. night, mid, morning)");
		string shiftType = Console.ReadLine();

		Console.WriteLine("Enter Start Time of shift: ");
		var startTimeInfo = Utility.GetDateTimeInput();

		Console.WriteLine("Enter End Time of shift: ");
		var endTimeInfo = Utility.GetDateTimeInput();
		while (endTimeInfo.dateTime < startTimeInfo.dateTime)
		{
			Console.WriteLine("End Time");
			endTimeInfo = Utility.GetDateTimeInput();
		}

		Shift shift = null;
		TimeSpan duration = Utility.CalculateDuration(endTimeInfo.dateTime, startTimeInfo.dateTime);
		if (shiftType != "")
		{
			shift = new Shift()
			{
				Id = id,
				Type = shiftType,
				StartTime = startTimeInfo.dateTime,
				EndTime = endTimeInfo.dateTime,
				Duration = duration
			};
		}
		else
		{
			shift = new Shift()
			{
				Id = id,
				StartTime = startTimeInfo.dateTime,
				EndTime = endTimeInfo.dateTime,
				Duration = duration
			};
		}


		using (var httpClient = new HttpClient())
		{
			try
			{
				HttpResponseMessage response = await httpClient.PutAsJsonAsync($"https://localhost:7204/api/shifts/{id}", shift);

				if (response.IsSuccessStatusCode)
				{
					Console.WriteLine("Shift successfully updated");
				}
				else
				{
					Console.WriteLine($"Failed to update shift. Status code: {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Server / API may be down or some other unexpected issue is going on: {ex.Message}");
				Console.ReadLine();
			}
		}
		Console.WriteLine("Press any key to return to main menu");
		Console.ReadLine();
	}
}
