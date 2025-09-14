namespace Common.Results.Extensions
{
    /// <summary>
    /// Методы расширения для работы с <see cref="Result"/>.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Сопоставление шаблонов для <see cref="Result"/>.
        /// </summary>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        /// <typeparam name="TResult">Тип результата.</typeparam>
        /// <param name="result">Исходный результат.</param>
        /// <param name="onSuccess">Функция для успешного случая.</param>
        /// <param name="onFailure">Функция для случая ошибки.</param>
        /// <returns>Результат соответствующей функции.</returns>
        public static TResult Match<TValue, TResult>(this Result<TValue> result, Func<TValue, TResult> onSuccess, Func<Exception, TResult> onFailure)
            => result.Success 
                ? onSuccess(result.Value) 
                : onFailure(result.Error!);

        /// <summary>
        /// Сопоставление шаблонов для <see cref="Result"/>.
        /// </summary>
        /// <typeparam name="TResult">Тип результата.</typeparam>
        /// <param name="result">Исходный результат.</param>
        /// <param name="onSuccess">Функция для успешного случая.</param>
        /// <param name="onFailure">Функция для случая ошибки.</param>
        /// <returns>Результат соответствующей функции.</returns>
        public static TResult Match<TResult>(this Result result, Func<TResult> onSuccess, Func<Exception, TResult> onFailure)
            => result.Success 
                ? onSuccess() 
                : onFailure(result.Error!);

        /// <summary>
        /// Выполнить действие при успехе.
        /// </summary>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        /// <param name="result">Исходный результат.</param>
        /// <param name="action">Действие для выполнения.</param>
        /// <returns>Исходный результат.</returns>
        public static Result<TValue> OnSuccess<TValue>(this Result<TValue> result, Action<TValue> action)
        {
            if (result.Success)
                action(result.Value);

            return result;
        }

        /// <summary>
        /// Выполнить действие при успехе.
        /// </summary>
        /// <param name="result">Исходный результат.</param>
        /// <param name="action">Действие для выполнения.</param>
        /// <returns>Исходный результат</returns>
        public static Result OnSuccess(this Result result, Action action)
        {
            if (result.Success)
                action();

            return result;
        }

        /// <summary>
        /// Преобразование значения при успехе.
        /// </summary>
        /// <typeparam name="TValue">Тип исходного значения.</typeparam>
        /// <typeparam name="TResult">Тип результирующего значения.</typeparam>
        /// <param name="result">Исходный результат.</param>
        /// <param name="mapper">Функция преобразования.</param>
        /// <returns>Результат с преобразованным значением или ошибкой.</returns>
        public static Result<TResult> Map<TValue, TResult>(this Result<TValue> result, Func<TValue, TResult> mapper)
            => result.Success
                ? Result<TResult>.Ok(mapper(result.Value))
                : Result<TResult>.Fail(result.Error);

        /// <summary>
        /// Преобразование void <see cref="Result"/> в <see cref="Result"/> с значением.
        /// </summary>
        /// <typeparam name="TResult">Тип результирующего значения.</typeparam>
        /// <param name="result">Исходный результат.</param>
        /// <param name="mapper">Функция создания значения.</param>
        /// <returns>Result со значением или ошибкой.</returns>
        public static Result<TResult> Map<TResult>(this Result result, Func<TResult> mapper)
            => result.Success
                ? Result<TResult>.Ok(mapper())
                : Result<TResult>.Fail(result.Error);
    }
}
