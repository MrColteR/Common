namespace Common.Results.Extensions
{
    /// <summary>
    /// Методы расширения для асинхронных операций с <see cref="Result"/>.
    /// </summary>
    public static class ResultAsyncExtensions
    {
        /// <summary>
        /// Асинхронное преобразование значения.
        /// </summary>
        /// <typeparam name="TValue">Тип исходного значения.</typeparam>
        /// <typeparam name="TResult">Тип результирующего значения.</typeparam>
        /// <param name="result">Исходный результат.</param>
        /// <param name="asyncMapper">Асинхронная функция преобразования.</param>
        /// <returns>Результат с преобразованным значением или ошибкой.</returns>
        public static async Task<Result<TResult>> MapAsync<TValue, TResult>(this Result<TValue> result, Func<TValue, Task<TResult>> asyncMapper)
            => result.Success
                ? Result<TResult>.Ok(await asyncMapper(result.Value))
                : Result<TResult>.Fail(result.Error);

        /// <summary>
        /// Асинхронное выполнение при успехе.
        /// </summary>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        /// <param name="result">Исходный результат.</param>
        /// <param name="asyncAction">Асинхронное действие.</param>
        /// <returns>Исходный Результат.</returns>
        public static async Task<Result<TValue>> OnSuccessAsync<TValue>(this Result<TValue> result, Func<TValue, Task> asyncAction)
        {
            if (result.Success)
                await asyncAction(result.Value);

            return result;
        }

        /// <summary>
        /// Асинхронно связывает результат с функцией, возвращающей новый <see cref="Result"/>.
        /// Если исходный результат успешен, применяет асинхронную функцию к значению и возвращает её результат.
        /// Если исходный результат неуспешен, возвращает ошибку без вызова функции.
        /// </summary>
        /// <typeparam name="TValue">Тип значения в исходном результате</typeparam>
        /// <typeparam name="TResult">Тип значения в результирующем результате</typeparam>
        /// <param name="result">Исходный результат для связывания</param>
        /// <param name="asyncBinder">Асинхронная функция, принимающая значение и возвращающая Task<Result<TResult>></param>
        /// <returns>Результат асинхронной операции связывания</returns>
        public static async Task<Result<TResult>> BindAsync<TValue, TResult>(this Result<TValue> result, Func<TValue, Task<Result<TResult>>> asyncBinder)
            => result.Success
                ? await asyncBinder(result.Value)
                : Result<TResult>.Fail(result.Error);

        /// <summary>
        /// Асинхронно связывает void результат с функцией, возвращающей <see cref="Result"/> с значением.
        /// Если исходный результат успешен, выполняет асинхронную функцию и возвращает её результат.
        /// Если исходный результат неуспешен, возвращает ошибку без вызова функции.
        /// </summary>
        /// <typeparam name="TResult">Тип значения в результирующем результате</typeparam>
        /// <param name="result">Исходный void результат для связывания</param>
        /// <param name="asyncBinder">Асинхронная функция</param>
        /// <returns>Результат асинхронной операции связывания</returns>
        public static async Task<Result<TResult>> BindAsync<TResult>(this Result result, Func<Task<Result<TResult>>> asyncBinder)
            => result.Success
                ? await asyncBinder()
                : Result<TResult>.Fail(result.Error);

        /// <summary>
        /// Обработка асинхронной операции.
        /// </summary>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        /// <param name="taskResult">Асинхронный результат.</param>
        /// <param name="asyncFunc">Асинхронная функция обработки.</param>
        /// <returns>Результат после асинхронной обработки.</returns>
        public static async Task<Result<TValue>> Then<TValue>(this Task<Result<TValue>> taskResult, Func<TValue, Task<Result<TValue>>> asyncFunc)
        {
            var result = await taskResult;

            return result.Success
                ? await asyncFunc(result.Value)
                : result;
        }
    }
}
