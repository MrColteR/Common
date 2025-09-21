namespace Common.Results
{
    /// <summary>
    /// Результат операции со значением.
    /// </summary>
    /// <typeparam name="TValue">Тип значения.</typeparam>
    public readonly struct Result<TValue>
    {
        /// <summary>
        /// Неявное преобразование значения в <see cref="Result"/>
        /// </summary>
        public static implicit operator Result<TValue>(TValue value)
            => new(true, value, null);

        /// <summary>
        /// Неявное преобразование исключения в <see cref="Result"/>
        /// </summary>
        public static implicit operator Result<TValue>(Exception error)
            => new(false, default!, error);

        /// <summary>
        /// Неявное преобразование <see cref="Result"/> в значение
        /// </summary>
        public static implicit operator TValue(Result<TValue> success)
            => success.Value;

        /// <summary>
        /// Успешный результат, содержащий значение.
        /// </summary>
        /// <param name="value">Значение результата.</param>
        /// <returns>Успешный результат.</returns>
        public static Result<TValue> Ok(TValue value)
            => new(true, value, null);

        /// <summary>
        /// Неуспешный результат с ошибкой.
        /// </summary>
        /// <param name="error">Исключение ошибки.</param>
        /// <returns>Неуспешный результат.</returns>
        public static Result<TValue> Fail(Exception? error = null)
            => new(false, default!, error);

        /// <summary>
        /// Неуспешный результат содержащий значение с ошибкой.
        /// </summary>
        /// <param name="value">Значение результата.</param>
        /// <param name="error">Исключение ошибки.</param>
        /// <returns>Неуспешный результат со значением.</returns>
        public static Result<TValue> Fail(TValue value, Exception? error = null)
            => new(false, value, error);

        /// <summary>
        /// Конструктор.
        /// </summary>
        private Result(bool success, TValue value, Exception? error)
        {
            Value = value;
            Success = success;
            Error = error;
        }

        /// <summary>
        /// Значение в случае успеха.
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// Успешность выполнения операции.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Ошибка при неуспешном выполнении операции.
        /// </summary>
        public Exception? Error { get; }

        /// <summary>
        /// Попытаться выполнить операцию.
        /// </summary>
        /// <param name="func">Функция для выполнения.</param>
        /// <returns>Результат с значением или ошибкой.</returns>
        public static Result<TValue> Get(Func<TValue> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            try
            {
                return Ok(func.Invoke());
            }
            catch (Exception ex)
            {
                return Fail(ex);
            }
        }

        /// <summary>
        /// Обработать ошибку в случае её наличия.
        /// </summary>
        /// <param name="errorHandler">Обработчик ошибки.</param>
        /// <returns>Текущий результат или с агрегированной ошибкой.</returns>
        public Result<TValue> WhenFailed(Action<Exception> errorHandler)
        {
            if (Error == null)
                return this;

            try
            {
                errorHandler?.Invoke(Error);
            }
            catch (Exception ex)
            {
                return new Result<TValue>(false, Value, new AggregateException(Error, ex));
            }

            return this;
        }

        /// <summary>
        /// Возбудить исключение, если есть ошибка выполнения операции.
        /// </summary>
        public void Throw()
        {
            if (Error != null)
                throw Error;
        }

        /// <summary>
        /// Задать значение по умолчанию.
        /// </summary>
        /// <param name="defaultValue">Значение по умолчанию.</param>
        /// <returns>Значение или значение по умолчанию.</returns>
        public TValue WithDefaultValue(TValue defaultValue)
            => Value != null 
                ? Value 
                : defaultValue;

        /// <summary>
        /// Назначить исключение по умолчанию, при неуспешном выполнении операции.
        /// </summary>
        /// <param name="exception">Исключение по умолчанию.</param>
        /// <returns>результат с ошибкой по умолчанию или текущий.</returns>
        public Result<TValue> WithDefaultError(Exception exception) 
            => (!Success && Error == null) 
                ? Fail(exception) 
                : this;
    }

    /// <summary>
    /// Результат выполнения операции.
    /// </summary>
    public readonly struct Result
    {
        /// <summary>
        /// Неявное преобразование исключения в результат
        /// </summary>
        public static implicit operator Result(Exception error) 
            => new(false, error);

        /// <summary>
        /// Неявное преобразование <see cref="Result"/> в <see cref="Task"/>.
        /// </summary>
        public static implicit operator Task<Result>(Result success)
            => Task.FromResult(success);

        /// <summary>
        /// Конструктор.
        /// </summary>
        private Result(bool success, Exception? error)
        {
            Success = success;
            Error = error;
        }

        /// <summary>
        /// Успешность выполнения операции.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Ошибка при неуспешном выполнении операции.
        /// </summary>
        public Exception? Error { get; }

        /// <summary>
        /// Успешное выполнение операции.
        /// </summary>
        /// <returns>Успешный результат.</returns>
        public static Result Ok()
            => new(true, null);

        /// <summary>
        /// Неуспешное выполнение операции.
        /// </summary>
        /// <param name="exception">Экземпляр ошибки.</param>
        /// <returns>Неуспешный результат.</returns>
        public static Result Fail(Exception? exception = null) 
            => new Result(false, exception);

        /// <summary>
        /// Попытаться выполнить операцию.
        /// </summary>
        /// <param name="func">Действие для выполнения.</param>
        /// <returns>Результат с результатом выполнения.</returns>
        public static Result Get(Action func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            try
            {
                func.Invoke();
                return Ok();
            }
            catch (Exception ex)
            {
                return Fail(ex);
            }
        }

        /// <summary>
        /// Обработать ошибку в случае её наличия.
        /// </summary>
        /// <param name="errorHandler">Обработчик ошибки.</param>
        /// <returns>Текущий результат или с агрегированной ошибкой.</returns>
        public Result WhenFailed(Action<Exception> errorHandler)
        {
            if (Error == null)
                return this;

            try
            {
                errorHandler?.Invoke(Error);
            }
            catch (Exception ex)
            {
                return Fail(new AggregateException(Error, ex));
            }

            return this;
        }

        /// <summary>
        /// Назначить исключение по умолчанию, для неуспешного выполнения операции без ошибки.
        /// </summary>
        /// <param name="exception">Исключение по умолчанию.</param>
        /// <returns>Результат с ошибкой по умолчанию или текущий.</returns>
        public Result WithDefaultError(Exception exception)
            => (!Success && Error == null)
                    ? Fail(exception)
                    : this;

        /// <summary>
        /// Возбудить исключение, если есть ошибка выполнения операции.
        /// </summary>
        public void Throw()
        {
            if (Error != null)
                throw Error;
        }
    }
}
