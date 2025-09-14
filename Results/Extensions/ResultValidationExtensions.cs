namespace Common.Results.Extensions
{
    /// <summary>
    /// Методы расширения для валидации <see cref="Result"/>.
    /// </summary>
    public static class ResultValidationExtensions
    {
        /// <summary>
        /// Валидация с коллекцией ошибок.
        /// </summary>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        /// <param name="result">Исходный результат.</param>
        /// <param name="validator">Функция валидации возвращающая ошибки.</param>
        /// <param name="errorFactory">Фабрика исключений.</param>
        /// <returns>Result с ошибками валидации или исходный результат.</returns>
        public static Result<TValue> Validate<TValue>(this Result<TValue> result, Func<TValue, IEnumerable<string>> validator, Func<string, Exception> errorFactory)
        {
            if (!result.Success) 
                return result;

            var errors = validator(result.Value).ToList();

            return errors.Any()
                ? Result<TValue>.Fail(new AggregateException(errors.Select(errorFactory)))
                : result;
        }

        /// <summary>
        /// Условная валидация.
        /// </summary>
        /// <typeparam name="TValue">Тип значения.</typeparam>
        /// <param name="result">Исходный результат.</param>
        /// <param name="condition">Условие валидации.</param>
        /// <param name="errorFactory">Фабрика исключений.</param>
        /// <returns>Результат с ошибкой или исходный результат.</returns>
        public static Result<TValue> When<TValue>(this Result<TValue> result, Func<TValue, bool> condition, Func<TValue, Exception> errorFactory)
        {
            if (!result.Success) return result;

            return condition(result.Value)
                ? result
                : Result<TValue>.Fail(errorFactory(result.Value));
        }

        /// <summary>
        /// Проверка на null.
        /// </summary>
        /// <typeparam name="TValue">Тип значения</typeparam>
        /// <param name="result">Исходный результат.</param>
        /// <param name="errorFactory">Фабрика исключений.</param>
        /// <returns>Результат с ошибкой или исходный результат.</returns>
        public static Result<TValue> NotNull<TValue>(this Result<TValue> result, Func<Exception> errorFactory) where TValue : class
            => result.Success && result.Value == null
                ? Result<TValue>.Fail(errorFactory())
                : result;
    }
}
