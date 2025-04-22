using Blazored.FluentValidation;
using modulum.Application.Requests.Dynamic.Create;
using modulum.Client.Infrastructure.FormValidators;
using modulum.Domain.Enums;
using modulum.Shared;
using modulum.Shared.Wrapper;

namespace Modulum.Client.Pages.Criacao
{
    public partial class Criacao
    {
        private FluentValidationValidator _fluentValidationValidator;
        private FormValidator _criacaoFormValidator = new();
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private string isvisivel = "invisible my-2";



        private List<CreateDynamicFieldRequest> _modelCampos = new();
        private ModelComposto _model = new();
        public class ModelComposto
        {
            public CreateDynamicTableRequest _modelTabela = new();
            public CreateDynamicFieldRequest _modelCampo = new();
        }

        private bool _loading = false;

        private readonly Dictionary<string, string> _tipos = new()
        {
            { "VARCHAR", "Texto" },
            { "INT", "Número" },
            { "DATE", "Data" },
            { "BIGINT", "Numero Grande" }
        }; // Esse cara deve conter os mesmo atributos que TypeColumnEnum se nao vai dar pau
        public string _tipoChapeu = string.Empty;

        private async void AdicionarCampo()
        {
            if (!string.IsNullOrWhiteSpace(_model._modelCampo.NomeCampoTela))
            {
                _modelCampos.Add(new CreateDynamicFieldRequest
                {
                    NomeCampoTela = _model._modelCampo.NomeCampoTela,
                    NomeCampoBase = _model._modelCampo.NomeCampoTela.Replace(" ", "_"),
                    Tipo = (TypeColumnEnum)Enum.Parse(typeof(TypeColumnEnum), _tipoChapeu),
                    Tamanho = _model._modelCampo.Tamanho,
                    IsPrimaryKey = false,
                    IsObrigatorio = false
                });
                _tipoChapeu = string.Empty;
                _model._modelCampo = new(); // limpa
            }
        }

        private void AddApiErrors(IResult response)
        {
            _criacaoFormValidator.ClearAllErrors();
            _criacaoFormValidator.DisplayAllErrors(response.Fields);
        }

        private void RemoverCampo(CreateDynamicFieldRequest campo)
        {
            _modelCampos.Remove(campo);
        }

        public async Task DoSalvarAsync()
        {
            _loading = true;
            _model._modelTabela.NomeTabela = _model._modelTabela.NomeTela?.Replace(" ", "_");
            _model._modelTabela.CampoPK = "Id";
            _modelCampos.Add(new CreateDynamicFieldRequest
            {
                NomeCampoTela = "Id",
                NomeCampoBase = "Id",
                IsObrigatorio = true,
                IsPrimaryKey = true,
                Tamanho = 10,
                Tipo = TypeColumnEnum.INT
            });
            _model._modelTabela.Campos = new();
            _model._modelTabela.Campos.AddRange(_modelCampos);
            var response = await _dynamicManager.CadastrarDynamic(_model._modelTabela);
            if (response != null)
            {
                if (response.Succeeded)
                {
                    _loading = false;
                    _navigationManager.NavigateTo("/System/concluido");
                }
                else
                {
                    // Tratar erro aqui
                    _loading = false;
                    return;
                }
            }
            else
            {
                _loading = false;
            }
        }
    }
}
