namespace Common.Results.Extensions
{
    /// <summary>
    /// Методы расширения для работы с Task и <see cref="Result"/>. 
    /// </summary>
    public static class ResultTaskExtensions
    {
        /// <summary>
        /// Преобразование Task в <see cref="Result"/>.
        /// </summary>
        /// <typeparam name="TValue">Тип возвращаемого значения.</typeparam>
        /// <param name="task">Асинхронная задача для выполнения.</param>
        /// <returns>результат с результатом или ошибкой.</returns>
        public static async Task<Result<TValue>> AsResult<TValue>(this Task<TValue> task)
        {
            try
            {
                return Result<TValue>.Ok(await task);
            }
            catch (Exception ex)
            {
                return Result<TValue>.Fail(ex);
            }
        }

        /// <summary>
        /// Преобразование Task в <see cref="Result"/>.
        /// </summary>
        /// <param name="task">Асинхронная задача для выполнения.</param>
        /// <returns>Результат с состоянием выполнения.</returns>
        public static async Task<Result> AsResult(this Task task)
        {
            try
            {
                await task;
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex);
            }
        }

        /// <summary>
        /// Безопасное выполнение асинхронной операции.
        /// </summary>
        /// <typeparam name="TValue">Тип возвращаемого значения.</typeparam>
        /// <param name="asyncFunc">Асинхронная функция для выполнения.</param>
        /// <param name="errorMapper">Функция для преобразования исключений.</param>
        /// <returns>Результат с результатом или преобразованной ошибкой.</returns>
        public static async Task<Result<TValue>> TryAsync<TValue>(
            Func<Task<TValue>> asyncFunc,
            Func<Exception, Exception>? errorMapper = null)
        {
            try
            {
                return Result<TValue>.Ok(await asyncFunc());
            }
            catch (Exception ex)
            {
                return Result<TValue>.Fail(errorMapper?.Invoke(ex) ?? ex);
            }
        }
    }
}
