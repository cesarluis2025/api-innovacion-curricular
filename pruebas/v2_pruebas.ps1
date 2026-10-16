# Pruebas de la v2 (los 12 criterios) y regresion de la v1.
# Ejecutar desde la carpeta que contiene docker-compose.yml:
#   powershell -ExecutionPolicy Bypass -File .\pruebas\v2_pruebas.ps1
# El script es solo ASCII a proposito: asi no depende de la codificacion de PowerShell 5.
# Cada corrida usa codigos de prueba distintos ($b), asi puede repetirse sin chocar.
# Los registros de prueba quedan RETIRADOS (activo = false), como manda el borrado logico.

$api = "http://localhost:8080"
$web = "http://localhost:8081"
$script:ok = 0; $script:mal = 0
$b = Get-Random -Minimum 100000 -Maximum 900000

function Req($metodo, $ruta, $cuerpo = $null, $base = $api) {
  $p = @{ Method = $metodo; Uri = "$base$ruta"; UseBasicParsing = $true }
  if ($null -ne $cuerpo) {
    $p.ContentType = "application/json; charset=utf-8"
    $p.Body = [Text.Encoding]::UTF8.GetBytes(($cuerpo | ConvertTo-Json -Depth 6))
  }
  try {
    $r = Invoke-WebRequest @p
    return @{ Codigo = [int]$r.StatusCode; Texto = $r.Content }
  } catch {
    $texto = ""
    try { $texto = (New-Object IO.StreamReader($_.Exception.Response.GetResponseStream())).ReadToEnd() } catch {}
    $c = 0; try { $c = [int]$_.Exception.Response.StatusCode.value__ } catch {}
    return @{ Codigo = $c; Texto = $texto }
  }
}
function Json($r) { try { return ($r.Texto | ConvertFrom-Json) } catch { return $null } }
function Sql($q) { (docker compose exec -T db psql -U postgres -d innovacion_curricular -t -A -c $q 2>&1 | Out-String).Trim() }
function Check($crit, $que, $obtenido, $esperado) {
  if ("$obtenido" -eq "$esperado") { $script:ok++; "  OK     [$crit] $que ($obtenido)" }
  else { $script:mal++; "  FALLA  [$crit] $que -> obtuvo '$obtenido', esperaba '$esperado'" }
}
function CheckTrue($crit, $que, $cond) {
  if ($cond) { $script:ok++; "  OK     [$crit] $que" } else { $script:mal++; "  FALLA  [$crit] $que" }
}
function Esta($lista, $prop, $valor) { return (@($lista | Where-Object { $_.$prop -eq $valor }).Count -gt 0) }

# Ciclo completo de una tabla de llave simple: crear, listar, editar, retirar, ya no aparece, sigue en la base.
function Ciclo($crit, $tabla, $col, $clave, $crear, $campo, $nuevo) {
  Check $crit "$tabla crear" (Req POST "/api/$tabla" $crear).Codigo 201
  CheckTrue $crit "$tabla aparece en el listado" (Esta (Json (Req GET "/api/$tabla")) $col $clave)
  $cambio = @{} + $crear; $cambio[$campo] = $nuevo
  Check $crit "$tabla editar" (Req PUT "/api/$tabla/$clave" $cambio).Codigo 200
  $leido = (Json (Req GET "/api/$tabla/$clave")).$campo
  Check $crit "$tabla el cambio quedo" $leido $nuevo
  Check $crit "$tabla retirar" (Req DELETE "/api/$tabla/$clave").Codigo 204
  Check $crit "$tabla ya no se lee" (Req GET "/api/$tabla/$clave").Codigo 404
  Check $crit "$tabla sigue en la base con activo = f" (Sql "SELECT activo FROM $tabla WHERE $col = $clave") "f"
}

"== 0. Servicios arriba"
Check 0 "API responde" (Req GET "/api/universidad").Codigo 200
Check 0 "Frontend responde" (Req GET "/" $null $web).Codigo 200

"== Base de datos"
Check 0 "funciones sp_*" (Sql "SELECT count(*) FROM pg_proc WHERE proname LIKE 'sp\_%'") 115
CheckTrue 4 "existe el disparador de facultad" ((Sql "SELECT count(*) FROM pg_trigger WHERE NOT tgisinternal AND tgrelid = 'programa'::regclass") -ge 1)

"== 3. Sin SQL escrito en C# (si la carpeta esta a mano)"
if (Test-Path ".\api_innovacion\Repositorios") {
  $sueltas = Select-String -CaseSensitive -Path ".\api_innovacion\Repositorios\*.cs" -Pattern "SELECT|INSERT|UPDATE|DELETE" | Where-Object { $_.Line -notmatch "FROM sp_" -and $_.Line.Trim() -notmatch "^//" }
  Check 3 "lineas de SQL que no llaman a un sp_" (@($sueltas).Count) 0
} else { "  (omitido: no encuentro api_innovacion\Repositorios aqui)" }

"== 12 (regresion v1) y 1: las 7 tablas de la v1, ciclo completo"
$areas = Json (Req GET "/api/area_conocimiento")
CheckTrue 12 "v1 criterio 1: el listado solo trae activos" (@($areas | Where-Object { $_.activo -ne $true }).Count -eq 0)
Ciclo 1 "universidad" "id" ($b+1) @{ id=$b+1; nombre="Uni ciclo"; tipo="Privada"; ciudad="Bogota" } "nombre" "Uni ciclo editada"
Ciclo 1 "area_conocimiento" "id" ($b+1) @{ id=$b+1; granArea="Gran area"; area="Area"; disciplina="Disciplina" } "area" "Area editada"
Ciclo 1 "aspecto_normativo" "id" ($b+1) @{ id=$b+1; tipo="Ley"; descripcion="Desc"; fuente="Fuente" } "fuente" "Fuente editada"
Ciclo 1 "aliado" "nit" ($b+1) @{ nit=$b+1; razonSocial="Aliado ciclo"; nombreContacto="Contacto"; correo="a@b.co"; telefono="3000000"; ciudad="Bogota" } "ciudad" "Cali"
Ciclo 1 "enfoque" "id" ($b+1) @{ id=$b+1; nombre="Enfoque ciclo"; descripcion="Desc" } "descripcion" "Desc editada"
Ciclo 1 "practica_estrategia" "id" ($b+1) @{ id=$b+1; tipo="Tipo"; nombre="Practica ciclo"; descripcion="Desc" } "nombre" "Practica editada"
Ciclo 1 "car_innovacion" "id" ($b+1) @{ id=$b+1; nombre="Car ciclo"; descripcion="Desc"; tipo="Tipo" } "descripcion" "Desc editada"
"  -- v1 criterios 2 y 3 (con los cambios de la v2: duplicado = 409; disciplina admite 130)"
Check 12 "v1 criterio 2: crear con un id existente -> 409" (Req POST "/api/universidad" @{ id=$b+1; nombre="x"; tipo="x"; ciudad="x" }).Codigo 409
Check 12 "v1 criterio 3: disciplina de 131 caracteres -> 400" (Req POST "/api/area_conocimiento" @{ id=$b+2; granArea="g"; area="a"; disciplina=("x" * 131) }).Codigo 400
Check 12 "v1 criterio 3 (nuevo limite): disciplina de 130 caracteres -> 201" (Req POST "/api/area_conocimiento" @{ id=$b+3; granArea="g"; area="a"; disciplina=("x" * 130) }).Codigo 201
Req DELETE "/api/area_conocimiento/$($b+3)" | Out-Null

"== Padres de prueba (quedan activos hasta el final)"
$U = $b+10; $F = $b+10; $P = $b+10; $RC = $b+10; $AA = $b+10; $AR = $b+10; $AS = $b+10; $EN = $b+10; $PE = $b+10; $CI = $b+10; $AL = $b+10
Check 2 "universidad padre" (Req POST "/api/universidad" @{ id=$U; nombre="Uni padre"; tipo="Privada"; ciudad="Bogota" }).Codigo 201
Check 2 "facultad padre" (Req POST "/api/facultad" @{ id=$F; nombre="Fac padre"; tipo="Facultad"; fechaFun="2020-01-01"; universidad=$U }).Codigo 201
Check 2 "programa padre" (Req POST "/api/programa" @{ id=$P; nombre="Programa padre"; tipo="Programa Academico"; nivel="Pregrado"; fechaCreacion="2020"; fechaCierre=$null; numeroCohortes="1"; cantGraduados="0"; fechaActualizacion="2020"; ciudad="Bogota"; facultad=$F }).Codigo 201
Check 2 "registro_calificado padre" (Req POST "/api/registro_calificado" @{ codigo=$RC; cantCreditos="160"; horaAcom="1"; horaInd="2"; metodologia="Presencial"; fechaInicio="2020-01-01"; fechaFin="2027-01-01"; duracionAnios="4"; duracionSemestres="8"; tipoTitulacion="Profesional"; programa=$P }).Codigo 201
Check 2 "activ_academica padre" (Req POST "/api/activ_academica" @{ id=$AA; nombre="Actividad padre"; numCreditos=3; tipo="Teorica"; areaFormacion="Basica"; hAcom=2; hIndep=4; idioma="Espanol"; espejo=0; disenio=$null }).Codigo 201
Req POST "/api/area_conocimiento" @{ id=$AR; granArea="g"; area="a"; disciplina="d" } | Out-Null
Req POST "/api/aspecto_normativo" @{ id=$AS; tipo="Ley"; descripcion="d"; fuente="f" } | Out-Null
Req POST "/api/enfoque" @{ id=$EN; nombre="Enfoque padre"; descripcion="d" } | Out-Null
Req POST "/api/practica_estrategia" @{ id=$PE; tipo="t"; nombre="Practica padre"; descripcion="d" } | Out-Null
Req POST "/api/car_innovacion" @{ id=$CI; nombre="Car padre"; descripcion="d"; tipo="t" } | Out-Null
Req POST "/api/aliado" @{ nit=$AL; razonSocial="Aliado padre"; nombreContacto="c"; correo="a@b.co"; telefono="3"; ciudad="Bogota" } | Out-Null

"== 2 y 6: las 8 tablas de detalle de la v2 (ciclo completo)"
$n = $b + 20
Ciclo 2 "facultad" "id" $n @{ id=$n; nombre="Fac ciclo"; tipo="Facultad"; fechaFun="2021-01-01"; universidad=$U } "nombre" "Fac ciclo editada"
Ciclo 2 "programa" "id" $n @{ id=$n; nombre="Prog ciclo"; tipo="Programa Academico"; nivel="Pregrado"; fechaCreacion="2021"; fechaCierre=$null; numeroCohortes="1"; cantGraduados="0"; fechaActualizacion="2021"; ciudad="Bogota"; facultad=$F } "ciudad" "Cali"
Ciclo 2 "acreditacion" "resolucion" $n @{ resolucion=$n; tipo="Alta calidad"; calificacion="A"; fechaInicio="2021"; fechaFin="2025"; programa=$P } "calificacion" "B"
Ciclo 2 "registro_calificado" "codigo" $n @{ codigo=$n; cantCreditos="150"; horaAcom="1"; horaInd="2"; metodologia="Presencial"; fechaInicio="2021-01-01"; fechaFin="2028-01-01"; duracionAnios="4"; duracionSemestres="8"; tipoTitulacion="Profesional"; programa=$P } "metodologia" "Virtual"
Ciclo 6 "activ_academica" "id" $n @{ id=$n; nombre="Act ciclo"; numCreditos=3; tipo="Teorica"; areaFormacion="Basica"; hAcom=2; hIndep=4; idioma="Espanol"; espejo=0; disenio=$null } "idioma" "Ingles"
Ciclo 2 "pasantia" "id" $n @{ id=$n; nombre="Pasantia ciclo"; pais="Colombia"; empresa="Empresa"; descripcion="Desc"; programa=$P } "empresa" "Otra"
Ciclo 2 "premio" "id" $n @{ id=$n; nombre="Premio ciclo"; descripcion="Desc"; fecha="2022-05-01"; entidadOtorgante="Entidad"; pais="Colombia"; programa=$P } "pais" "Peru"

"== 6: FK opcional y regla del espejo"
$r = Req POST "/api/activ_academica" @{ id=$b+30; nombre="Sin diseno"; numCreditos=3; tipo="Teorica"; areaFormacion="Basica"; hAcom=2; hIndep=4; idioma="Espanol"; espejo=0; disenio=$null }
Check 6 "activ_academica con disenio = null" $r.Codigo 201
$ent = (Json $r).entidadEspejo
Check 6 "espejo = 0 guarda No aplica" $ent "No aplica"
Req DELETE "/api/activ_academica/$($b+30)" | Out-Null
Check 6 "espejo = 1 sin entidad -> 400" (Req POST "/api/activ_academica" @{ id=$b+31; nombre="Espejo"; numCreditos=3; tipo="Teorica"; areaFormacion="Basica"; hAcom=2; hIndep=4; idioma="Espanol"; espejo=1; disenio=$null }).Codigo 400

"== 8: las 8 tablas puente"
Check 8 "programa_ac asignar" (Req POST "/api/programa_ac" @{ programa=$P; areaConocimiento=$AR }).Codigo 201
Check 8 "programa_ac repetir -> 409" (Req POST "/api/programa_ac" @{ programa=$P; areaConocimiento=$AR }).Codigo 409
Check 8 "programa_ac leer" (Req GET "/api/programa_ac/$P/$AR").Codigo 200
Check 8 "programa_ac retirar" (Req DELETE "/api/programa_ac/$P/$AR").Codigo 204
Check 8 "programa_ac ya no se lee" (Req GET "/api/programa_ac/$P/$AR").Codigo 404
Check 10 "programa_ac sigue en la base con activo = f" (Sql "SELECT activo FROM programa_ac WHERE programa = $P AND area_conocimiento = $AR") "f"
Check 8 "programa_ac reactivar -> 201" (Req POST "/api/programa_ac" @{ programa=$P; areaConocimiento=$AR }).Codigo 201
Check 8 "programa_ac retirar de nuevo" (Req DELETE "/api/programa_ac/$P/$AR").Codigo 204
$puentes = @(
  @{ t="programa_pe"; a=$P; b2=$PE; cuerpo=@{ programa=$P; practicaEstrategia=$PE } },
  @{ t="programa_ci"; a=$P; b2=$CI; cuerpo=@{ programa=$P; carInnovacion=$CI } },
  @{ t="an_programa"; a=$AS; b2=$P; cuerpo=@{ aspectoNormativo=$AS; programa=$P } },
  @{ t="enfoque_rc"; a=$EN; b2=$RC; cuerpo=@{ enfoque=$EN; registroCalificado=$RC } },
  @{ t="aa_rc"; a=$AA; b2=$RC; cuerpo=@{ activAcademicasIdcurso=$AA; registroCalificadoCodigo=$RC; componente="Basico"; semestre="1" }; put=@{ activAcademicasIdcurso=$AA; registroCalificadoCodigo=$RC; componente="Profesional"; semestre="2" }; campo="componente"; nuevo="Profesional" },
  @{ t="docente_departamento"; a=12345; b2=$P; cuerpo=@{ docente=12345; departamento=$P; dedicacion="TC"; modalidad="Presencial"; fechaIngreso="2020-01-01"; fechaSalida=$null }; put=@{ docente=12345; departamento=$P; dedicacion="MT"; modalidad="Presencial"; fechaIngreso="2020-01-01"; fechaSalida=$null }; campo="dedicacion"; nuevo="MT" },
  @{ t="alianza"; a=$AL; b2=$P; cuerpo=@{ aliado=$AL; departamento=$P; fechaInicio="2022-01-01"; fechaFin=$null; docente=$null }; put=@{ aliado=$AL; departamento=$P; fechaInicio="2022-01-01"; fechaFin="2024-01-01"; docente=$null }; campo="fechaFin"; nuevo=$null }
)
foreach ($x in $puentes) {
  $t = $x.t; $ruta = "/api/$t/$($x.a)/$($x.b2)"
  Check 8 "$t asignar" (Req POST "/api/$t" $x.cuerpo).Codigo 201
  Check 8 "$t repetir -> 409" (Req POST "/api/$t" $x.cuerpo).Codigo 409
  if ($x.put) { Check 8 "$t editar" (Req PUT $ruta $x.put).Codigo 200 }
  Check 8 "$t retirar" (Req DELETE $ruta).Codigo 204
  Check 8 "$t ya no se lee" (Req GET $ruta).Codigo 404
}

"== 7: hijo con padre inexistente -> 409 (nunca 500)"
$prog = @{ id=$b+40; nombre="Hijo huerfano"; tipo="Programa Academico"; nivel="Pregrado"; fechaCreacion="2026"; fechaCierre=$null; numeroCohortes="1"; cantGraduados="0"; fechaActualizacion="2026"; ciudad="Bogota"; facultad=9999999 }
Check 7 "programa con facultad inexistente" (Req POST "/api/programa" $prog).Codigo 409
Check 7 "facultad con universidad inexistente" (Req POST "/api/facultad" @{ id=$b+41; nombre="x"; tipo="x"; fechaFun="2020-01-01"; universidad=9999999 }).Codigo 409
Check 7 "acreditacion con programa inexistente" (Req POST "/api/acreditacion" @{ resolucion=$b+42; tipo="x"; calificacion="x"; fechaInicio="x"; fechaFin="x"; programa=9999999 }).Codigo 409
Check 7 "puente con padre inexistente" (Req POST "/api/programa_ac" @{ programa=9999999; areaConocimiento=$AR }).Codigo 409
Check 7 "llave repetida (programa padre otra vez)" (Req POST "/api/programa" @{ id=$P; nombre="Dup"; tipo="x"; nivel="x"; fechaCreacion="x"; fechaCierre=$null; numeroCohortes="1"; cantGraduados="0"; fechaActualizacion="x"; ciudad="x"; facultad=$F }).Codigo 409

"== 4: el disparador cuenta solo (facultad $F, hoy con el programa padre)"
Check 4 "total_programas inicial" (Sql "SELECT total_programas FROM facultad WHERE id = $F") 1
Req POST "/api/programa" @{ id=$b+50; nombre="Prog disparador"; tipo="x"; nivel="x"; fechaCreacion="x"; fechaCierre=$null; numeroCohortes="1"; cantGraduados="0"; fechaActualizacion="x"; ciudad="x"; facultad=$F } | Out-Null
Check 4 "al agregar un programa sube a 2" (Sql "SELECT total_programas FROM facultad WHERE id = $F") 2
$F2 = $b + 11
Req POST "/api/facultad" @{ id=$F2; nombre="Fac destino"; tipo="Facultad"; fechaFun="2020-01-01"; universidad=$U } | Out-Null
Req PUT "/api/programa/$($b+50)" @{ nombre="Prog disparador"; tipo="x"; nivel="x"; fechaCreacion="x"; fechaCierre=$null; numeroCohortes="1"; cantGraduados="0"; fechaActualizacion="x"; ciudad="x"; facultad=$F2 } | Out-Null
Check 4 "al mover el programa corrige la facultad de origen" (Sql "SELECT total_programas FROM facultad WHERE id = $F") 1
Check 4 "...y la de destino" (Sql "SELECT total_programas FROM facultad WHERE id = $F2") 1
Req DELETE "/api/programa/$($b+50)" | Out-Null
Check 4 "al retirar baja" (Sql "SELECT total_programas FROM facultad WHERE id = $F2") 0

"== 9: maestro-detalle, varias filas en UNA peticion"
$lista = Json (Req GET "/api/universidad/$U/facultades")
CheckTrue 9 "detalle de la universidad lista su facultad" (Esta $lista "id" $F)
$lote = @{ renglones = @(
  @{ resolucion=$b+61; tipo="Alta calidad"; calificacion="A"; fechaInicio="2022"; fechaFin="2026" },
  @{ resolucion=$b+62; tipo="Alta calidad"; calificacion="B"; fechaInicio="2022"; fechaFin="2026" },
  @{ resolucion=$b+63; tipo="Alta calidad"; calificacion="C"; fechaInicio="2022"; fechaFin="2026" }) }
Check 9 "lote de 3 acreditaciones -> 201" (Req POST "/api/programa/$P/acreditaciones/lote" $lote).Codigo 201
Check 9 "las 3 quedaron" (@((Json (Req GET "/api/programa/$P/acreditaciones")) | Where-Object { $_.resolucion -ge ($b+61) -and $_.resolucion -le ($b+63) }).Count) 3
$malo = @{ renglones = @(
  @{ resolucion=$b+64; tipo="x"; calificacion="A"; fechaInicio="2022"; fechaFin="2026" },
  @{ resolucion=$b+61; tipo="x"; calificacion="A"; fechaInicio="2022"; fechaFin="2026" }) }
Check 9 "lote con un renglon repetido -> 409" (Req POST "/api/programa/$P/acreditaciones/lote" $malo).Codigo 409
CheckTrue 9 "todo o nada: el renglon bueno NO se guardo" (-not (Esta (Json (Req GET "/api/programa/$P/acreditaciones")) "resolucion" ($b+64)))

"== 10 y B1: retiro logico y regla de maestro con detalle"
Check 10 "retirar una facultad con programas activos -> 409" (Req DELETE "/api/facultad/$F").Codigo 409
$m = (Req DELETE "/api/facultad/$F").Texto
CheckTrue 10 "el mensaje explica que tiene registros activos" ($m -match "registros activos")
Check 10 "retirar una universidad con facultades activas -> 409" (Req DELETE "/api/universidad/$U").Codigo 409

"== 5, 6, 9 y 11: interfaz (se pide cada pagina al frontend)"
$paginas = @("Universidad","AreaConocimiento","AspectoNormativo","Aliado","Enfoque","PracticaEstrategia","CarInnovacion",
  "Facultad","Programa","Acreditacion","RegistroCalificado","ActivAcademica","Pasantia","Premio",
  "ProgramaAc","ProgramaPe","ProgramaCi","AnPrograma","EnfoqueRc","AaRc","DocenteDepartamento","Alianza")
$jerga = 0; $caidas = 0
foreach ($pg in $paginas) {
  foreach ($suf in @("", "/Crear")) {
    $w = Req GET "/$pg$suf" $null $web
    if ($w.Codigo -ne 200) { $caidas++; "  FALLA  [1/2] /$pg$suf respondio $($w.Codigo)" }
    else { $h = [Net.WebUtility]::HtmlDecode(($w.Texto -replace '(?s)<script.*?</script>', '')); if ($h -cmatch '\b(PUT|PATCH|422|FK)\b') { $jerga++; "  FALLA  [11] jerga en /$pg$suf" } }
  }
}
Check "1/2" "las 22 tablas abren listado y formulario (44 paginas)" $caidas 0
Check 11 "ninguna pantalla muestra PUT, PATCH, 422 ni FK" $jerga 0
$crearProg = [Net.WebUtility]::HtmlDecode((Req GET "/Programa/Crear" $null $web).Texto)
$f101 = (Json (Req GET "/api/facultad/101")).nombre
CheckTrue 5 "Nuevo programa: el desplegable de Facultad trae nombres" ($crearProg.Contains(">$f101<"))
CheckTrue 5 "...y no se digita el codigo (no hay campo de numero para la facultad)" (-not ($crearProg -match 'name="Registro.Facultad"[^>]*type="number"'))
CheckTrue 5 "una facultad retirada ya no se ofrece" (-not $crearProg.Contains(">Fac ciclo editada<"))
$crearAct = [Net.WebUtility]::HtmlDecode((Req GET "/ActivAcademica/Crear" $null $web).Texto)
CheckTrue 6 "Nueva actividad: el programa de diseno ofrece (ninguna)" ($crearAct.Contains("(ninguna)"))
$det = [Net.WebUtility]::HtmlDecode((Req GET "/Programa/Detalle/$P" $null $web).Texto)
foreach ($s in @("Acreditaciones","Registros calificados","Actividades acad","Pasant","Premios")) { CheckTrue 9 "el detalle del programa muestra '$s'" ($det.Contains($s)) }
foreach ($pg in @("Universidad","Facultad")) { Check 9 "/$pg/Detalle abre" (Req GET "/$pg/Detalle/$(if ($pg -eq 'Universidad') { $U } else { $F })" $null $web).Codigo 200 }

"== Limpieza: se retira lo que se creo (primero los hijos)"
foreach ($t in @("acreditacion")) { foreach ($k in @(($b+61),($b+62),($b+63))) { Req DELETE "/api/$t/$k" | Out-Null } }
Req DELETE "/api/aa_rc/$AA/$RC" | Out-Null
Req DELETE "/api/registro_calificado/$RC" | Out-Null
Req DELETE "/api/activ_academica/$AA" | Out-Null
Check 10 "retirar el programa padre" (Req DELETE "/api/programa/$P").Codigo 204
Check 10 "retirar la facultad padre" (Req DELETE "/api/facultad/$F").Codigo 204
Req DELETE "/api/facultad/$F2" | Out-Null
Check 10 "retirar la universidad padre" (Req DELETE "/api/universidad/$U").Codigo 204
foreach ($t in @(@("area_conocimiento",$AR),@("aspecto_normativo",$AS),@("enfoque",$EN),@("practica_estrategia",$PE),@("car_innovacion",$CI),@("aliado",$AL))) { Req DELETE "/api/$($t[0])/$($t[1])" | Out-Null }

"== Datos de referencia intactos"
Check 0 "programas activos (suman lo mismo que el disparador)" (Sql "SELECT count(*) FROM programa WHERE activo") (Sql "SELECT sum(total_programas) FROM facultad")

""
"RESULTADO: $script:ok correctas, $script:mal con falla"
if ($script:mal -eq 0) { "TODO EN VERDE" } else { "HAY FALLAS: copie las lineas FALLA y envielas" }
