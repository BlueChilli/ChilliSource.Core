#region License

/*
Licensed to Blue Chilli Technology Pty Ltd and the contributors under the MIT License (the "License").
You may not use this file except in compliance with the License.
See the LICENSE file in the project root for more information.
*/

#endregion

using System;
using Xunit;

using ChilliSource.Mobile.Core;
using System.Threading.Tasks;

namespace Tests
{
	public class OperationResultTests
	{
		class Dummy
		{
            public string Name { get; set; }
		}

		[Fact]
		public void AsSuccess_ShouldSetFieldsCorrectly()
		{
			var result = OperationResult.AsSuccess("test message");
			Assert.True(result.IsSuccessful);
			Assert.False(result.IsCancelled);
			Assert.Null(result.Exception);
			Assert.Equal("test message", result.Message);

			var genericResult = OperationResult<Dummy>.AsSuccess(new Dummy());
			Assert.True(genericResult.IsSuccessful);
			Assert.False(genericResult.IsCancelled);
			Assert.Null(genericResult.Exception);
			Assert.Null(genericResult.Message);
			Assert.Equal(typeof(Dummy), genericResult.Result.GetType());
		}

		[Fact]
		public void AsCancel_ShouldSetFieldsCorrectly()
		{
			var result = OperationResult.AsCancel();
			Assert.False(result.IsSuccessful);
			Assert.True(result.IsCancelled);
			Assert.Null(result.Exception);
			Assert.Null(result.Message);

			var genericResult = OperationResult<Dummy>.AsCancel();
			Assert.False(genericResult.IsSuccessful);
			Assert.True(genericResult.IsCancelled);
			Assert.Null(genericResult.Exception);
			Assert.Null(genericResult.Message);
			Assert.Null(genericResult.Result);
		}

		[Fact]
		public void AsFailure_ShouldSetFieldsCorrectly_WhenGivenException()
		{
			var result = OperationResult.AsFailure(new Exception("test exception"));
			Assert.False(result.IsSuccessful);
			Assert.False(result.IsCancelled);
			Assert.Equal("test exception", result.Message);
			Assert.Equal("test exception", result.Exception.Message);

			var genericResult = OperationResult<Dummy>.AsFailure(new Exception("test exception"), new Dummy());
			Assert.False(genericResult.IsSuccessful);
			Assert.False(genericResult.IsCancelled);
			Assert.Equal("test exception", genericResult.Message);
			Assert.Equal("test exception", genericResult.Exception.Message);
			Assert.Equal(typeof(Dummy), genericResult.Result.GetType());
		}

		[Fact]
		public void AsFailure_ShouldSetFieldsCorrectly_WhenGivenMessage()
		{
			var result = OperationResult.AsFailure("test exception");
			Assert.False(result.IsSuccessful);
			Assert.False(result.IsCancelled);
			Assert.Equal("test exception", result.Message);
			Assert.Equal("test exception", result.Exception.Message);

			var genericResult = OperationResult<Dummy>.AsFailure("test exception", new Dummy());
			Assert.False(genericResult.IsSuccessful);
			Assert.False(genericResult.IsCancelled);
			Assert.Equal("test exception", genericResult.Message);
			Assert.Equal("test exception", genericResult.Exception.Message);
			Assert.Equal(typeof(Dummy), genericResult.Result.GetType());
		}

		[Fact]
		public async Task OnSuccessAsync_ShouldBeCalled_WhenReturnsSuccessResult()
		{            			
			var hasSuccessExecuted = false;
			var hasFailureExecuted = false;
            var hasAlwaysExecuted = false;

			var result = await Task.FromResult(OperationResult<string>.AsSuccess("ok"))
				.OnSuccessAsync((r) =>
				{
					hasSuccessExecuted = true;
					return Task.FromResult(OperationResult<string>.AsSuccess("ok"));
				})
				.OnFailureAsync(() =>
				{
					hasFailureExecuted = true;
					return Task.Delay(0);
				})
				.AlwaysAsync(() =>
				{
					hasAlwaysExecuted = true;
					return Task.Delay(0);
				});

			Assert.True(hasSuccessExecuted);
			Assert.False(hasFailureExecuted);
			Assert.True(hasAlwaysExecuted);
			Assert.True(result.IsSuccessful);
			Assert.Equal("ok", result.Result);
		}

		[Fact]
		public void Combine_ShouldFail_WhenResultConsistsAtLeastOneErrorResult()
		{
			var res1 = OperationResult.AsFailure("failed");
			var res2 = OperationResult.AsSuccess();
			var res3 = OperationResult.AsFailure("test");

			var r = OperationResult.Combine(res1, res2, res3);

			Assert.False(r.IsSuccessful);
			Assert.True(r.Message.Contains("failed"));
		}

		[Fact]
		public void Combine_ShouldSucceed_WhenResultConsistsAllSuccess()
		{
			var res1 = OperationResult.AsSuccess();
			var res2 = OperationResult.AsSuccess();
			var res3 = OperationResult.AsSuccess();

			var r = OperationResult.Combine(res1, res2, res3);

			Assert.True(r.IsSuccessful);
			Assert.False(r.Message.Contains("failed"));
		}

		[Fact]
		public async Task WaitForResult_ShouldSucceed_WhenResultConsistsAllSuccess()
		{
			var task = Task.FromResult(true);
			OperationResult result = await task
				.WaitForResult();

			Assert.True(result.IsSuccessful);
		}
	}
}
