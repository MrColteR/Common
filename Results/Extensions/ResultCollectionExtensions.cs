namespace Common.Results.Extensions
{
    /// <summary>
    /// Методы расширения для работы с коллекциями <see cref="Result"/>.
    /// </summary>
    public static class ResultCollectionExtensions
    {
        /// <summary>
        /// Объединяет коллекцию <see cref="Result"/> в один <see cref="Result"/>.
        /// </summary>
        /// <typeparam name="TValue">Тип значений в результате.</typeparam>
        /// <param name="results">Коллекция результатов для объединения.</param>
        /// <returns>Результат с коллекцией значений или <see cref="AggregateException"/>.</returns>
        public static Result<IEnumerable<TValue>> Combine<TValue>(this IEnumerable<Result<TValue>> results)
        {
            var resultsList = results.ToList();
            var errors = resultsList.Where(r => !r.Success).Select(r => r.Error).ToList();

            return errors.Any()
                ? Result<IEnumerable<TValue>>.Fail(new AggregateException(errors!))
                : Result<IEnumerable<TValue>>.Ok(resultsList.Select(r => r.Value));
        }

        /// <summary>
        /// Асинхронно обрабатывает коллекцию значений.
        /// </summary>
        /// <typeparam name="TValue">Тип исходных значений.</typeparam>
        /// <typeparam name="TResult">Тип результирующих значений.</typeparam>
        /// <param name="values">Коллекция значений для обработки.</param>
        /// <param name="asyncFunc">Асинхронная функция преобразования.</param>
        /// <returns>Результат с коллекцией результатов или ошибками.</returns>
        public static async Task<Result<IEnumerable<TResult>>> TraverseAsync<TValue, TResult>(
            this IEnumerable<TValue> values, Func<TValue,
            Task<Result<TResult>>> asyncFunc)
        {
            var results = await Task.WhenAll(values.Select(asyncFunc));

            return results.Combine();
        }

        /// <summary>
        /// Разделяет коллекцию <see cref="Result"/> на успешные значения и ошибки.
        /// </summary>
        /// <typeparam name="TValue">Тип значений в результате.</typeparam>
        /// <param name="results">Коллекция результатов для разделения.</param>
        /// <returns>Кортеж с успешными значениями и ошибками.</returns>
        public static (IEnumerable<TValue> Success, IEnumerable<Exception?> Errors) Partition<TValue>(this IEnumerable<Result<TValue>> results)
        {
            var resultsList = results.ToList();
            var success = resultsList.Where(r => r.Success).Select(r => r.Value);
            var errors = resultsList.Where(r => !r.Success).Select(r => r.Error);

            return (success, errors);
        }
    }
}
